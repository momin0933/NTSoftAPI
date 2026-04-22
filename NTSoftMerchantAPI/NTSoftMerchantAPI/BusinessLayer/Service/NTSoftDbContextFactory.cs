using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NTSoftMerchantAPI.BusinessLayer.TenantService;


namespace NTSoftMerchantAPI.BusinessLayer.Service
{
    public class NTSoftDbContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerFactory _loggerFactory;

        public NTSoftDbContextFactory(IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public NTSoftDbContext CreateDbContext()
        {
            var tenant = _httpContextAccessor.HttpContext?.Items["Tenant"] as Tenant;
            var logger = _loggerFactory.CreateLogger<NTSoftDbContext>();

                if (tenant?.ConnectionString == null)
                {
                    logger?.LogWarning("No tenant or connection string found in HttpContext");
                    return CreateEmptyContext(logger);
                }

                return CreateContextWithConnection(tenant.ConnectionString, logger);
            }

            private NTSoftDbContext CreateEmptyContext(ILogger<NTSoftDbContext>? logger)
            {
                var optionsBuilder = new DbContextOptionsBuilder<NTSoftDbContext>();
                // No connection string configured - using default behavior
            
            return new NTSoftDbContext(optionsBuilder.Options, _httpContextAccessor, logger!);
        }

        private NTSoftDbContext CreateContextWithConnection(string connectionString, ILogger<NTSoftDbContext>? logger)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NTSoftDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new NTSoftDbContext(optionsBuilder.Options, _httpContextAccessor, logger!);
        }
    }
}
