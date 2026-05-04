namespace HRAPI.BusinessLayer.TenantService
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ITenantStore _tenantStore;


        public TenantProvider(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ITenantStore tenantStore)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _tenantStore = tenantStore;
        }

        public string GetConnectionString()
        {
            ////var tenant = _httpContextAccessor.HttpContext?.Items["Tenant"] as Tenant;
            //var tenant = _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value;

            //if (tenant != null && !string.IsNullOrEmpty(tenant.ConnectionString))
            //{
            //    return tenant.ConnectionString;
            //}

            //return _configuration.GetConnectionString("DefaultConnection");
            // 1. Token theke tenantId nao
           
            string tenantId = null;

            // 1. LOGIN TIME (NO TOKEN)
            tenantId = _httpContextAccessor.HttpContext?.Request?.Headers["TenantId"].FirstOrDefault();

            // 2. AFTER LOGIN (TOKEN)
            if (string.IsNullOrEmpty(tenantId))
            {
                tenantId = _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value;
            }

            if (!string.IsNullOrEmpty(tenantId))
            {
                // 2. DB / cache theke tenant info nao
                var tenant = _tenantStore.GetTenant(tenantId);

                if (tenant != null && !string.IsNullOrEmpty(tenant.ConnectionString))
                {
                    return tenant.ConnectionString;
                }
            }

            // 3. fallback
            return _configuration.GetConnectionString("DefaultConnection");
        }

        public string GetTenantId()
        {
            var tenant = _httpContextAccessor.HttpContext?.Items["Tenant"] as Tenant;
            return tenant?.TenantKey;
        }
    }
}