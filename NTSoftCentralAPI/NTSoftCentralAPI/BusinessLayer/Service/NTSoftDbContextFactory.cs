
using Microsoft.EntityFrameworkCore;
using NTSoftCentralAPI.BusinessLayer.TenantService;


namespace NTSoftCentralAPI.BusinessLayer.Service
{
    public class NTSoftDbContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public NTSoftDbContextFactory(IHttpContextAccessor httpContextAccessor,IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public NTSoftDbContext CreateDbContext()
        {
            var tenant = _httpContextAccessor.HttpContext?.Items["Tenant"] as Tenant;

            string connectionString;

            if (tenant != null && !string.IsNullOrEmpty(tenant.ConnectionString))
            {
                connectionString = tenant.ConnectionString;
            }
            else
            {
                // ⚠ fallback (careful use)
                connectionString = _configuration.GetConnectionString("DefaultConnection");
            }

            var optionsBuilder = new DbContextOptionsBuilder<NTSoftDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new NTSoftDbContext(optionsBuilder.Options);
        }
    }
}
