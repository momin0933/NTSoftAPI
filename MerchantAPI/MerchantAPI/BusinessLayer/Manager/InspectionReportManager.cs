using System.Collections;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MerchantAPI.BusinessLayer.Interface;
using MerchantAPI.Models;
using MerchantAPI.Models.Inspections;
using MerchantAPI.BusinessLayer.TenantService;
using Microsoft.AspNetCore.Http;

namespace MerchantAPI.BusinessLayer.Manager
{
    public class InspectionReportManager : IInspectionReportManager
    {
        private const string SP = "SP_InspectionReport";
        private const string SP_DETAIL = "SP_InspectionReportDetail";

        private readonly ITenantStore _tenantStore;
        private readonly IHttpContextAccessor _http;
        private string? _connectionString;

        public InspectionReportManager(ITenantStore tenantStore, IHttpContextAccessor http)
        {
            _tenantStore = tenantStore ?? throw new ArgumentNullException(nameof(tenantStore));
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        // The database of the tenant sent in the "TenantId" header (e.g. NR0933),
        // looked up in CentralDB through SP_TenantList (cached by TenantStore).
        private string ConnectionString => _connectionString ??= ResolveConnectionString();

        // 1) "TenantId" header  2) tenant claim of the Bearer token (same token the other modules use)
        private string? ReadTenantId()
        {
            var ctx = _http.HttpContext;
            if (ctx == null) return null;

            var h = ctx.Request.Headers["TenantId"].FirstOrDefault()?.Trim();
            if (!string.IsNullOrEmpty(h)) return h;

            var auth = ctx.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(auth) || !auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return null;
            try
            {
                var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(auth.Substring(7).Trim());
                var names = new[] { "TenantId", "tenantId", "tenant_id", "tenant", "TenantKey" };
                return jwt.Claims.FirstOrDefault(c => names.Contains(c.Type, StringComparer.OrdinalIgnoreCase))?.Value?.Trim();
            }
            catch { return null; }
        }

        private string ResolveConnectionString()
        {
            var tenantId = ReadTenantId();
            if (string.IsNullOrEmpty(tenantId))
                throw new ArgumentException("TenantId was not found in the request header or the login token.");

            var tenant = _tenantStore.GetTenant(tenantId);
            if (tenant == null || string.IsNullOrWhiteSpace(tenant.ConnectionString))
                throw new ArgumentException($"Unknown tenant '{tenantId}'.");

            return tenant.ConnectionString;
        }

        // ------------------------------------------------------------------
        // SP parameters
        // ------------------------------------------------------------------

        /// <summary>Only these types become SP parameters — lists and objects never do.</summary>
        private static bool IsSimple(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal)
                   || t == typeof(DateTime) || t == typeof(DateTimeOffset) || t == typeof(Guid);
        }

        /// <summary>
        /// @Action + every simple property of the model (property name = SP parameter name).
        /// Never pass a DynamicParameters here: it has a "ParameterNames" list, and Dapper would
        /// send it as @ParameterNames1, @ParameterNames2… ("is not a parameter for procedure").
        /// </summary>
        private static DynamicParameters Params(int action, object? model = null)
        {
            var p = new DynamicParameters();
            p.Add("Action", action);
            if (model == null) return p;

            if (model is DynamicParameters)
                throw new ArgumentException("Pass a model or use Sp(...), not DynamicParameters, to Params().");

            foreach (var prop in model.GetType().GetProperties())
            {
                if (!prop.CanRead || prop.GetIndexParameters().Length > 0) continue;
                if (!IsSimple(prop.PropertyType)) continue;               // skips List<>, IEnumerable, nested objects
                p.Add(prop.Name, prop.GetValue(model));
            }
            return p;
        }

        /// <summary>Explicit parameters for the short calls: Sp(2, ("Id", id)).</summary>
        private static DynamicParameters Sp(int action, params (string Name, object? Value)[] values)
        {
            var p = new DynamicParameters();
            p.Add("Action", action);
            foreach (var (name, value) in values)
            {
                if (value is IEnumerable && value is not string)
                    throw new ArgumentException($"Parameter '{name}' must be a single value, not a list.");
                p.Add(name, value);
            }
            return p;
        }

        // ------------------------------------------------------------------
        // 1. Get All (paginated)
        // ------------------------------------------------------------------
        public async Task<(List<InspectionReport> Data, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate, string? search)
        {
            using var conn = new SqlConnection(ConnectionString);

            var p = Sp(1,
                ("PageNumber", pageNumber < 1 ? 1 : pageNumber),
                ("PageSize", pageSize < 1 ? 50 : pageSize),
                ("FromDate", fromDate),
                ("ToDate", toDate),
                ("Search", string.IsNullOrWhiteSpace(search) ? null : search.Trim()));

            using var multi = await conn.QueryMultipleAsync(SP, p, commandType: CommandType.StoredProcedure);
            var data = (await multi.ReadAsync<InspectionReport>()).ToList();
            var total = await multi.ReadFirstOrDefaultAsync<int?>() ?? data.Count;
            return (data, total);
        }

        // ------------------------------------------------------------------
        // 2. Get By Id (report + all its rows and photos)   <- used by Edit and PDF
        // ------------------------------------------------------------------
        public async Task<InspectionReport?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            var report = await conn.QueryFirstOrDefaultAsync<InspectionReport>(
                SP, Sp(2, ("Id", id)), commandType: CommandType.StoredProcedure);
            if (report == null) return null;

            using var multi = await conn.QueryMultipleAsync(
                SP_DETAIL, Sp(1, ("ReportId", id)), commandType: CommandType.StoredProcedure);

            report.RemarkList = (await multi.ReadAsync<InspectionRemark>()).ToList();
            report.Defects = (await multi.ReadAsync<InspectionDefect>()).ToList();
            report.Checks = (await multi.ReadAsync<InspectionCheck>()).ToList();
            report.Cartons = (await multi.ReadAsync<InspectionCarton>()).ToList();
            report.Measures = (await multi.ReadAsync<InspectionMeasure>()).ToList();
            var photos = (await multi.ReadAsync<InspectionPhoto>()).ToList();
            report.Attachments = (await multi.ReadAsync<InspectionAttachment>()).ToList();

            // put each photo under its defect / check / report
            foreach (var d in report.Defects) d.Photos = photos.Where(x => x.DefectId == d.Id).ToList();
            foreach (var c in report.Checks) c.Photos = photos.Where(x => x.CheckId == c.Id).ToList();
            report.Photos = photos.Where(x => x.DefectId == null && x.CheckId == null).ToList();

            return report;
        }

        // ------------------------------------------------------------------
        // 3. Insert (report + rows in one transaction)
        // ------------------------------------------------------------------
        public async Task<int> AddAsync(InspectionReport report, string? userName)
        {
            Validate(report);
            report.EntryBy = userName;

            using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                var id = await conn.ExecuteScalarAsync<int>(SP, Params(3, report), tx, commandType: CommandType.StoredProcedure);
                await SaveDetailsAsync(conn, tx, id, report, userName);
                tx.Commit();
                return id;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // ------------------------------------------------------------------
        // 4. Update (report + rows replaced, one transaction)
        // ------------------------------------------------------------------
        public async Task<bool> UpdateAsync(InspectionReport report, string? userName)
        {
            Validate(report);
            report.UpdateBy = userName;

            using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                var affected = await conn.ExecuteScalarAsync<int>(SP, Params(4, report), tx, commandType: CommandType.StoredProcedure);
                if (affected == 0)
                {
                    tx.Rollback();
                    return false;
                }

                // old rows -> IsActive = 0, then insert the current ones
                await conn.ExecuteScalarAsync<int>(SP_DETAIL, Sp(2, ("ReportId", report.Id), ("UpdateBy", userName)), tx, commandType: CommandType.StoredProcedure);
                await SaveDetailsAsync(conn, tx, report.Id, report, userName);

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // ------------------------------------------------------------------
        // 5. Delete (soft)
        // ------------------------------------------------------------------
        public async Task<bool> DeleteAsync(int id, string? userName)
        {
            using var conn = new SqlConnection(ConnectionString);
            var affected = await conn.ExecuteScalarAsync<int>(
                SP, Sp(5, ("Id", id), ("UpdateBy", userName)), commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        // ------------------------------------------------------------------
        // Insert every child row (SP_InspectionReportDetail Action 3..9)
        // ------------------------------------------------------------------
        private static async Task SaveDetailsAsync(SqlConnection conn, IDbTransaction tx, int reportId, InspectionReport report, string? userName)
        {
            async Task<int> Insert(int action, Base row, int sortOrder)
            {
                row.EntryBy = userName;
                var p = Params(action, row);       // a model, never a DynamicParameters
                p.Add("ReportId", reportId);
                p.Add("SortOrder", sortOrder);
                return await conn.ExecuteScalarAsync<int>(SP_DETAIL, p, tx, commandType: CommandType.StoredProcedure);
            }

            async Task InsertPhotos(List<InspectionPhoto> photos, string section, int? defectId, int? checkId)
            {
                var i = 0;
                foreach (var ph in (photos ?? new()).Where(x => !string.IsNullOrWhiteSpace(x.FilePath)))
                {
                    ph.Section = section;
                    ph.DefectId = defectId;
                    ph.CheckId = checkId;
                    await Insert(8, ph, i++);
                }
            }

            var order = 0;
            foreach (var r in (report.RemarkList ?? new()).Where(x => !string.IsNullOrWhiteSpace(x.Text)))
                await Insert(3, r, order++);

            order = 0;
            foreach (var d in (report.Defects ?? new()).Where(x => !string.IsNullOrWhiteSpace(x.Name)))
            {
                var defectId = await Insert(4, d, order++);
                await InsertPhotos(d.Photos, "Defect", defectId, null);
            }

            order = 0;
            foreach (var c in (report.Checks ?? new()).Where(x => !string.IsNullOrWhiteSpace(x.CheckKey)))
            {
                var checkId = await Insert(5, c, order++);
                await InsertPhotos(c.Photos, "Check", null, checkId);
            }

            order = 0;
            foreach (var c in report.Cartons ?? new())
                await Insert(6, c, order++);

            order = 0;
            foreach (var m in (report.Measures ?? new()).Where(x => !string.IsNullOrWhiteSpace(x.Name)))
                await Insert(7, m, order++);

            foreach (var g in (report.Photos ?? new()).GroupBy(x => x.Section is "Start" or "Measurement" ? x.Section! : "General"))
                await InsertPhotos(g.ToList(), g.Key, null, null);

            order = 0;
            foreach (var a in (report.Attachments ?? new()).Where(x => !string.IsNullOrWhiteSpace(x.FilePath)))
                await Insert(9, a, order++);
        }

        // Inspection reference, product name and date are NOT NULL in the table
        private static void Validate(InspectionReport report)
        {
            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(report.InspRef)) missing.Add("inspection reference");
            if (string.IsNullOrWhiteSpace(report.ProductName)) missing.Add("product name");
            if (report.InspDate == null) missing.Add("inspection date");
            if (missing.Count > 0) throw new ArgumentException("Please fill in: " + string.Join(", ", missing));
        }
    }
}