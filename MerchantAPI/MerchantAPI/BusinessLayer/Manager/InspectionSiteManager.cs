using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using MerchantAPI.BusinessLayer.Interface;
using MerchantAPI.BusinessLayer.TenantService;
using MerchantAPI.Models.Inspections;

namespace MerchantAPI.BusinessLayer.Manager
{
    public class InspectionSiteManager : IInspectionSite
    {
        private const string SP = "SP_InspectionSite";

        private readonly ITenantStore _tenantStore;
        private readonly IHttpContextAccessor _http;
        private string? _connectionString;

        public InspectionSiteManager(ITenantStore tenantStore, IHttpContextAccessor http)
        {
            _tenantStore = tenantStore ?? throw new ArgumentNullException(nameof(tenantStore));
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        private string ConnectionString => _connectionString ??= ResolveConnectionString();

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

        // Every parameter is added by name — nothing is read from an object's properties.
        private static DynamicParameters Sp(int action, params (string Name, object? Value)[] values)
        {
            var p = new DynamicParameters();
            p.Add("Action", action);
            foreach (var (name, value) in values) p.Add(name, value);
            return p;
        }

        // 50001 / 50002 are raised by the procedure (duplicate name / name missing); 2601 / 2627 = unique index hit
        private static bool IsRuleError(SqlException ex) => ex.Number is 50001 or 50002 or 2601 or 2627;
        private static string RuleMessage(SqlException ex) =>
            ex.Number is 2601 or 2627 ? "A site with this name already exists." : ex.Message;

        public async Task<List<InspectionSite>> GetAllAsync(string? search)
        {
            using var conn = new SqlConnection(ConnectionString);
            var rows = await conn.QueryAsync<InspectionSite>(
                SP, Sp(1, ("Search", string.IsNullOrWhiteSpace(search) ? null : search.Trim())),
                commandType: CommandType.StoredProcedure);
            return rows.ToList();
        }

        public async Task<InspectionSite?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(ConnectionString);
            return await conn.QueryFirstOrDefaultAsync<InspectionSite>(
                SP, Sp(2, ("Id", id)), commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AddAsync(InspectionSite site, string? userName)
        {
            Validate(site);
            using var conn = new SqlConnection(ConnectionString);
            try
            {
                return await conn.ExecuteScalarAsync<int>(
                    SP,
                    Sp(3,
                        ("SiteName", site.SiteName), ("SiteAddress", site.SiteAddress),
                        ("SitePhone", site.SitePhone), ("SiteRep", site.SiteRep),
                        ("Remarks", site.Remarks), ("EntryBy", userName)),
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (IsRuleError(ex))
            {
                throw new ArgumentException(RuleMessage(ex));
            }
        }

        public async Task<bool> UpdateAsync(InspectionSite site, string? userName)
        {
            Validate(site);
            using var conn = new SqlConnection(ConnectionString);
            try
            {
                var affected = await conn.ExecuteScalarAsync<int>(
                    SP,
                    Sp(4,
                        ("Id", site.Id),
                        ("SiteName", site.SiteName), ("SiteAddress", site.SiteAddress),
                        ("SitePhone", site.SitePhone), ("SiteRep", site.SiteRep),
                        ("Remarks", site.Remarks), ("UpdateBy", userName)),
                    commandType: CommandType.StoredProcedure);
                return affected > 0;
            }
            catch (SqlException ex) when (IsRuleError(ex))
            {
                throw new ArgumentException(RuleMessage(ex));
            }
        }

        public async Task<bool> DeleteAsync(int id, string? userName)
        {
            using var conn = new SqlConnection(ConnectionString);
            var affected = await conn.ExecuteScalarAsync<int>(
                SP, Sp(5, ("Id", id), ("UpdateBy", userName)), commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        private static void Validate(InspectionSite site)
        {
            if (string.IsNullOrWhiteSpace(site.SiteName))
                throw new ArgumentException("Site name is required.");
        }
    }
}