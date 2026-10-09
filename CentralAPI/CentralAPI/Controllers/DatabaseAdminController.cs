using CentralAPI.BusinessLayer.TenantService;
using CentralAPI.Models;
using System.Data;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CentralAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "AdminOnly")]
    public class DatabaseAdminController : ControllerBase
    {
        private static readonly Regex DbNameRule = new(@"^[A-Za-z][A-Za-z0-9_]{1,99}$");
        private const int LongTimeoutSeconds = 3600;

        // Object types shown in the custom picker
        // U = table, V = view, P = stored procedure, FN/IF/TF = functions
        private const string ObjectTypeFilter = "('U','V','P','FN','IF','TF')";

        // Same filter for listing and for cleanup, so both see exactly the same objects.
        // Diagram support objects (sysdiagrams, sp_*diagram*) are hidden.
        private const string UserObjectsSql = @"
            SELECT o.object_id, s.name AS SchemaName, o.name AS ObjectName, RTRIM(o.type) AS ObjType,
                   CASE WHEN o.type = 'U' THEN
                        (SELECT ISNULL(SUM(p.rows), 0) FROM sys.partitions p
                         WHERE p.object_id = o.object_id AND p.index_id IN (0, 1))
                   END AS RowsCount
            FROM sys.objects o
            JOIN sys.schemas s ON s.schema_id = o.schema_id
            WHERE o.is_ms_shipped = 0
              AND o.type IN " + ObjectTypeFilter + @"
              AND NOT EXISTS (SELECT 1 FROM sys.extended_properties ep
                              WHERE ep.major_id = o.object_id AND ep.minor_id = 0
                                AND ep.name = 'microsoft_database_tools_support')
            ORDER BY o.type, s.name, o.name;";

        // ---------------------------------------------------------------
        // Connection to master on the tenant's server (same login)
        // ---------------------------------------------------------------
        private string GetMasterConnectionString()
        {
            if (HttpContext.Items["Tenant"] is not Tenant tenant)
                throw new InvalidOperationException("Tenant not resolved for this request.");

            var builder = new SqlConnectionStringBuilder(tenant.ConnectionString)
            {
                InitialCatalog = "master",
                Encrypt = true,
                TrustServerCertificate = true,
                ConnectTimeout = 30
            };
            return builder.ConnectionString;
        }

        // ---------------------------------------------------------------
        // GET api/DatabaseAdmin/databases
        // ---------------------------------------------------------------
        [HttpGet("databases")]
        public async Task<IActionResult> GetDatabases()
        {
            const string sql = @"
                SELECT name FROM sys.databases
                WHERE database_id > 4 AND source_database_id IS NULL AND state_desc = 'ONLINE'
                ORDER BY name;";
            try
            {
                var list = new List<string>();
                await using var conn = new SqlConnection(GetMasterConnectionString());
                await conn.OpenAsync();
                await using var cmd = new SqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    list.Add(reader.GetString(0));

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ---------------------------------------------------------------
        // GET api/DatabaseAdmin/objects?database=XYZ
        // Tables, views, procedures and functions of one database,
        // plus which object depends on which (for the picker warnings).
        // ---------------------------------------------------------------
        [HttpGet("objects")]
        public async Task<IActionResult> GetObjects([FromQuery] string database)
        {
            if (string.IsNullOrWhiteSpace(database))
                return BadRequest(new { message = "Database name is required." });

            try
            {
                await using var conn = new SqlConnection(GetMasterConnectionString());
                await conn.OpenAsync();

                if (!await DatabaseExists(conn, database, userOnly: true))
                    return BadRequest(new { message = $"Database '{database}' was not found." });

                conn.ChangeDatabase(database);

                var objects = (await ReadUserObjects(conn)).Select(o => new
                {
                    id = o.Id,
                    schema = o.Schema,
                    name = o.Name,
                    type = TypeLabel(o.Type),
                    rows = o.Rows
                }).ToList();

                // code dependencies (view → table, proc → view, trigger → table counts as its table, ...)
                // and foreign keys (child table → parent table)
                const string depSql = @"
                    SELECT DISTINCT
                        CASE WHEN ro.parent_object_id <> 0 THEN ro.parent_object_id ELSE d.referencing_id END AS FromId,
                        d.referenced_id AS ToId,
                        'code' AS Kind
                    FROM sys.sql_expression_dependencies d
                    JOIN sys.objects ro ON ro.object_id = d.referencing_id
                    WHERE d.referenced_id IS NOT NULL
                    UNION
                    SELECT DISTINCT fk.parent_object_id, fk.referenced_object_id, 'fk'
                    FROM sys.foreign_keys fk
                    WHERE fk.parent_object_id <> fk.referenced_object_id;";

                var ids = objects.Select(o => o.id).ToHashSet();
                var deps = new List<object>();
                await using (var cmd = new SqlCommand(depSql, conn))
                await using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        int from = r.GetInt32(0), to = r.GetInt32(1);
                        if (from != to && ids.Contains(from) && ids.Contains(to))
                            deps.Add(new { from, to, kind = r.GetString(2) });
                    }
                }

                return Ok(new { objects, dependencies = deps });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ---------------------------------------------------------------
        // POST api/DatabaseAdmin/clone
        // ---------------------------------------------------------------
        [HttpPost("clone")]
        public async Task<IActionResult> CloneDatabase([FromBody] CloneDatabaseRequest req)
        {
            var newName = req?.NewDatabaseName?.Trim();
            var source = req?.SourceDatabaseName?.Trim();
            var isCustom = string.Equals(req?.Mode, "custom", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(newName) || !DbNameRule.IsMatch(newName))
                return BadRequest(new { message = "Database name must start with a letter and use only letters, numbers and underscore (2–100 characters)." });

            if (string.IsNullOrEmpty(source))
                return BadRequest(new { message = "Select a source database." });

            if (isCustom && (req.KeepObjectIds == null || req.KeepObjectIds.Count == 0))
                return BadRequest(new { message = "Select at least one object to keep." });

            await using var conn = new SqlConnection(GetMasterConnectionString());
            await conn.OpenAsync();

            if (!await DatabaseExists(conn, source, userOnly: true))
                return BadRequest(new { message = $"Source database '{source}' was not found." });

            if (await DatabaseExists(conn, newName, userOnly: false))
                return Conflict(new { message = $"A database named '{newName}' already exists." });

            var (dataPath, logPath, backupPath) = await GetServerPaths(conn);
            var backupFile = $"{backupPath}{source}_clone_{DateTime.Now:yyyyMMddHHmmss}.bak";

            bool restoreStarted = false;
            try
            {
                // 1) COPY_ONLY backup of the source
                await using (var backup = new SqlCommand(
                    "BACKUP DATABASE @src TO DISK = @file WITH COPY_ONLY, INIT, FORMAT;", conn))
                {
                    backup.CommandTimeout = LongTimeoutSeconds;
                    backup.Parameters.AddWithValue("@src", source);
                    backup.Parameters.AddWithValue("@file", backupFile);
                    await backup.ExecuteNonQueryAsync();
                }

                // 2) logical files inside the backup
                var files = await ReadBackupFileList(conn, backupFile);

                // 3) restore under the new name, moving every file
                var sql = new StringBuilder("RESTORE DATABASE @newName FROM DISK = @file WITH ");
                var restore = new SqlCommand { Connection = conn, CommandTimeout = LongTimeoutSeconds };
                restore.Parameters.AddWithValue("@newName", newName);
                restore.Parameters.AddWithValue("@file", backupFile);

                for (int i = 0; i < files.Count; i++)
                {
                    var f = files[i];
                    var folder = f.Type == "L" ? logPath : dataPath;
                    var physical = f.Type == "S"
                        ? $"{folder}{newName}_fs{f.FileId}"
                        : $"{folder}{newName}_{f.FileId}{f.Extension}";

                    sql.Append($"MOVE @l{i} TO @p{i}, ");
                    restore.Parameters.AddWithValue($"@l{i}", f.LogicalName);
                    restore.Parameters.AddWithValue($"@p{i}", physical);
                }
                sql.Append("RECOVERY;");
                restore.CommandText = sql.ToString();

                restoreStarted = true;
                await using (restore)
                    await restore.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                if (restoreStarted)
                    await TryDropDatabase(conn, newName);

                return StatusCode(500, new { message = "Clone failed: " + ex.Message });
            }

            // 4) custom mode: remove everything that was not selected
            //    (the new database already exists; problems here are reported as warnings)
            int removedCount = 0;
            var warnings = new List<string>();
            if (isCustom)
            {
                try
                {
                    (removedCount, warnings) = await RemoveUnselectedObjects(conn, newName, req.KeepObjectIds.ToHashSet());
                }
                catch (Exception ex)
                {
                    warnings.Add("Cleanup stopped early: " + ex.Message);
                }
            }

            return Ok(new
            {
                message = isCustom
                    ? $"Database '{newName}' created from '{source}' with your selected objects."
                    : $"Database '{newName}' created from '{source}'.",
                database = newName,
                mode = isCustom ? "custom" : "full",
                removedCount,
                warnings,
                backupFile
            });
        }

        // ================= custom clone cleanup =================

        private static async Task<(int removed, List<string> warnings)> RemoveUnselectedObjects(
            SqlConnection conn, string dbName, HashSet<int> keepIds)
        {
            var warnings = new List<string>();
            conn.ChangeDatabase(dbName);

            var all = await ReadUserObjects(conn);
            var toDrop = all.Where(o => !keepIds.Contains(o.Id)).ToList();
            var dropTableIds = toDrop.Where(o => o.Type == "U").Select(o => o.Id).ToHashSet();

            // a) foreign keys that point to a table we are going to remove
            //    (otherwise SQL Server refuses to drop the referenced table)
            const string fkSql = @"
                SELECT fk.name, s.name, t.name, fk.parent_object_id, fk.referenced_object_id
                FROM sys.foreign_keys fk
                JOIN sys.tables t  ON t.object_id = fk.parent_object_id
                JOIN sys.schemas s ON s.schema_id = t.schema_id;";

            var fks = new List<(string Fk, string Schema, string Table, int ParentId, int RefId)>();
            await using (var cmd = new SqlCommand(fkSql, conn))
            await using (var r = await cmd.ExecuteReaderAsync())
            {
                while (await r.ReadAsync())
                    fks.Add((r.GetString(0), r.GetString(1), r.GetString(2), r.GetInt32(3), r.GetInt32(4)));
            }

            foreach (var fk in fks.Where(f => dropTableIds.Contains(f.RefId)))
            {
                try
                {
                    await Exec(conn, $"ALTER TABLE {Q(fk.Schema)}.{Q(fk.Table)} DROP CONSTRAINT {Q(fk.Fk)};");
                    if (!dropTableIds.Contains(fk.ParentId))
                        warnings.Add($"Foreign key {fk.Fk} on table {fk.Schema}.{fk.Table} was removed because the table it points to was not kept.");
                }
                catch (SqlException ex)
                {
                    warnings.Add($"Could not remove foreign key {fk.Fk}: {ex.Message}");
                }
            }

            // b) drop objects: code first, tables last.
            //    Retry a few passes so schema-bound chains resolve in any order.
            var rank = new Dictionary<string, int> { ["P"] = 0, ["V"] = 1, ["FN"] = 2, ["IF"] = 2, ["TF"] = 2, ["U"] = 3 };
            var pending = toDrop.OrderBy(o => rank[o.Type]).ToList();
            var lastError = new Dictionary<int, string>();
            int removed = 0;

            for (int pass = 0; pass < 5 && pending.Count > 0; pass++)
            {
                var failed = new List<DbObject>();
                foreach (var o in pending)
                {
                    try
                    {
                        await Exec(conn, $"DROP {DropKeyword(o.Type)} {Q(o.Schema)}.{Q(o.Name)};");
                        removed++;
                    }
                    catch (SqlException ex)
                    {
                        failed.Add(o);
                        lastError[o.Id] = ex.Message;
                    }
                }
                if (failed.Count == pending.Count) break; // no progress
                pending = failed;
            }

            foreach (var o in pending)
                warnings.Add($"Could not remove {TypeLabel(o.Type)} {o.Schema}.{o.Name}: {lastError[o.Id]}");

            conn.ChangeDatabase("master");
            return (removed, warnings);
        }

        // ================= helpers =================

        private record DbObject(int Id, string Schema, string Name, string Type, long? Rows);

        private static async Task<List<DbObject>> ReadUserObjects(SqlConnection conn)
        {
            var list = new List<DbObject>();
            await using var cmd = new SqlCommand(UserObjectsSql, conn);
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new DbObject(
                    r.GetInt32(0),
                    r.GetString(1),
                    r.GetString(2),
                    r.GetString(3),
                    r.IsDBNull(4) ? null : Convert.ToInt64(r.GetValue(4))));
            }
            return list;
        }

        private static string TypeLabel(string type) => type switch
        {
            "U" => "table",
            "V" => "view",
            "P" => "procedure",
            _ => "function"
        };

        private static string DropKeyword(string type) => type switch
        {
            "U" => "TABLE",
            "V" => "VIEW",
            "P" => "PROCEDURE",
            _ => "FUNCTION"
        };

        // QUOTENAME equivalent. Names come from sys catalog views, never from the request.
        private static string Q(string name) => "[" + name.Replace("]", "]]") + "]";

        private static async Task Exec(SqlConnection conn, string sql)
        {
            await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = LongTimeoutSeconds };
            await cmd.ExecuteNonQueryAsync();
        }

        private static async Task<bool> DatabaseExists(SqlConnection conn, string name, bool userOnly)
        {
            var sql = "SELECT COUNT(1) FROM sys.databases WHERE name = @n"
                      + (userOnly ? " AND database_id > 4 AND source_database_id IS NULL" : "");
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", name);
            return (int)await cmd.ExecuteScalarAsync() > 0;
        }

        private static async Task<(string data, string log, string backup)> GetServerPaths(SqlConnection conn)
        {
            const string sql = @"
                SELECT CAST(SERVERPROPERTY('InstanceDefaultDataPath')   AS nvarchar(512)),
                       CAST(SERVERPROPERTY('InstanceDefaultLogPath')    AS nvarchar(512)),
                       CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS nvarchar(512));";

            await using var cmd = new SqlCommand(sql, conn);
            await using var r = await cmd.ExecuteReaderAsync();
            await r.ReadAsync();

            string data = EnsureSlash(r.IsDBNull(0) ? null : r.GetString(0));
            string log = EnsureSlash(r.IsDBNull(1) ? data : r.GetString(1));
            string backup = EnsureSlash(r.IsDBNull(2) ? data : r.GetString(2));

            if (string.IsNullOrEmpty(data))
                throw new InvalidOperationException("Could not read the SQL Server default data path.");

            return (data, log, backup);
        }

        private static string EnsureSlash(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            return path.EndsWith("\\") || path.EndsWith("/") ? path : path + "\\";
        }

        private record BackupFile(string LogicalName, string Type, long FileId, string Extension);

        private static async Task<List<BackupFile>> ReadBackupFileList(SqlConnection conn, string backupFile)
        {
            var result = new List<BackupFile>();
            await using var cmd = new SqlCommand("RESTORE FILELISTONLY FROM DISK = @file;", conn)
            {
                CommandTimeout = LongTimeoutSeconds
            };
            cmd.Parameters.AddWithValue("@file", backupFile);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                var logical = r["LogicalName"].ToString();
                var physical = r["PhysicalName"].ToString();
                var type = r["Type"].ToString();
                var fileId = Convert.ToInt64(r["FileId"]);

                var dot = physical.LastIndexOf('.');
                var ext = dot >= 0 ? physical.Substring(dot) : (type == "L" ? ".ldf" : ".ndf");

                result.Add(new BackupFile(logical, type, fileId, ext));
            }
            return result;
        }

        private static async Task TryDropDatabase(SqlConnection conn, string name)
        {
            try
            {
                if (conn.State != ConnectionState.Open) await conn.OpenAsync();
                await using var cmd = new SqlCommand(
                    "IF DB_ID(@n) IS NOT NULL BEGIN DECLARE @sql nvarchar(400) = N'DROP DATABASE ' + QUOTENAME(@n); EXEC(@sql); END", conn);
                cmd.Parameters.AddWithValue("@n", name);
                await cmd.ExecuteNonQueryAsync();
            }
            catch { /* best-effort cleanup */ }
        }
    }
}
