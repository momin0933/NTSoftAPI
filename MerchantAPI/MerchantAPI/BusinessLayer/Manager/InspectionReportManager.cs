using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MerchantAPI.BusinessLayer.Interface;
using MerchantAPI.Models;
using MerchantAPI.Models.Inspections;

namespace MerchantAPI.BusinessLayer.Manager
{
    public class InspectionReportManager : IInspectionReportManager
    {
        private const string SP = "SP_InspectionReport";
        private const string SP_DETAIL = "SP_InspectionReportDetail";

        private readonly string _connectionString;

        public InspectionReportManager(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        // SP parameters from a model: @Action + every simple property (lists are skipped).
        // Property names = SP parameter names, so nothing has to be listed by hand.
        private static DynamicParameters Params(int action, object? model = null)
        {
            var p = new DynamicParameters();
            p.Add("Action", action);
            if (model == null) return p;

            foreach (var prop in model.GetType().GetProperties())
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime))
                    p.Add(prop.Name, prop.GetValue(model));
            }
            return p;
        }

        // 1. Get All (paginated)
        public async Task<(List<InspectionReport> Data, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate, string? search)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = Params(1);
            p.Add("PageNumber", pageNumber < 1 ? 1 : pageNumber);
            p.Add("PageSize", pageSize < 1 ? 50 : pageSize);
            p.Add("FromDate", fromDate);
            p.Add("ToDate", toDate);
            p.Add("Search", search);

            using var multi = await conn.QueryMultipleAsync(SP, p, commandType: CommandType.StoredProcedure);
            var data = (await multi.ReadAsync<InspectionReport>()).ToList();
            var total = await multi.ReadFirstAsync<int>();
            return (data, total);
        }

        // 2. Get By Id (report + all its rows and photos)
        public async Task<InspectionReport?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            var report = await conn.QueryFirstOrDefaultAsync<InspectionReport>(
                SP, Params(2, new { Id = id }), commandType: CommandType.StoredProcedure);
            if (report == null) return null;

            using var multi = await conn.QueryMultipleAsync(
                SP_DETAIL, Params(1, new { ReportId = id }), commandType: CommandType.StoredProcedure);

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

        // 3. Insert (report + rows in one transaction)
        public async Task<int> AddAsync(InspectionReport report, string? userName)
        {
            Validate(report);
            report.EntryBy = userName;

            using var conn = new SqlConnection(_connectionString);
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

        // 4. Update (report + rows replaced, one transaction)
        public async Task<bool> UpdateAsync(InspectionReport report, string? userName)
        {
            Validate(report);
            report.UpdateBy = userName;

            using var conn = new SqlConnection(_connectionString);
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
                await conn.ExecuteScalarAsync<int>(SP_DETAIL, Params(2, new { ReportId = report.Id, UpdateBy = userName }), tx, commandType: CommandType.StoredProcedure);
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

        // 5. Delete (soft)
        public async Task<bool> DeleteAsync(int id, string? userName)
        {
            using var conn = new SqlConnection(_connectionString);
            var affected = await conn.ExecuteScalarAsync<int>(
                SP, Params(5, new { Id = id, UpdateBy = userName }), commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        // ------------------------------------------------------------------
        // Insert every child row (SP_InspectionReportDetail Action 3..9)
        private static async Task SaveDetailsAsync(SqlConnection conn, IDbTransaction tx, int reportId, InspectionReport report, string? userName)
        {
            async Task<int> Insert(int action, Base row, int sortOrder)
            {
                row.EntryBy = userName;
                var p = Params(action, row);
                p.Add("ReportId", reportId);
                p.Add("SortOrder", sortOrder);
                return await conn.ExecuteScalarAsync<int>(SP_DETAIL, p, tx, commandType: CommandType.StoredProcedure);
            }

            async Task InsertPhotos(List<InspectionPhoto> photos, string section, int? defectId, int? checkId)
            {
                var i = 0;
                foreach (var ph in photos.Where(x => !string.IsNullOrWhiteSpace(x.FilePath)))
                {
                    ph.Section = section;
                    ph.DefectId = defectId;
                    ph.CheckId = checkId;
                    await Insert(8, ph, i++);
                }
            }

            var order = 0;
            foreach (var r in report.RemarkList.Where(x => !string.IsNullOrWhiteSpace(x.Text)))
                await Insert(3, r, order++);

            order = 0;
            foreach (var d in report.Defects.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
            {
                var defectId = await Insert(4, d, order++);
                await InsertPhotos(d.Photos, "Defect", defectId, null);
            }

            order = 0;
            foreach (var c in report.Checks.Where(x => !string.IsNullOrWhiteSpace(x.CheckKey)))
            {
                var checkId = await Insert(5, c, order++);
                await InsertPhotos(c.Photos, "Check", null, checkId);
            }

            order = 0;
            foreach (var c in report.Cartons)
                await Insert(6, c, order++);

            order = 0;
            foreach (var m in report.Measures.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
                await Insert(7, m, order++);

            foreach (var g in report.Photos.GroupBy(x => x.Section is "Start" or "Measurement" ? x.Section! : "General"))
                await InsertPhotos(g.ToList(), g.Key, null, null);

            order = 0;
            foreach (var a in report.Attachments.Where(x => !string.IsNullOrWhiteSpace(x.FilePath)))
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