using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using NTSoftMerchantAPI.BusinessLayer.TenantService;
using NTSoftMerchantAPI.Models;


namespace NTSoftMerchantAPI.BusinessLayer.Service
{
    public partial class NTSoftDbContext : DbContext
    {
        private readonly string _connectionString;
        //SQLConnectionString _connectionStringService = new SQLConnectionString();
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NTSoftDbContext()
        {
            // _connectionString = cnString;
        }
        public NTSoftDbContext(DbContextOptions<NTSoftDbContext> options, IHttpContextAccessor httpContextAccessor)
    : base(options)
        {
            //_connectionString = _connectionStringService.GetConnectionString("default");
            _httpContextAccessor = httpContextAccessor;
        }
        public NTSoftDbContext(DbContextOptions<NTSoftDbContext> options)
            : base(options)
        {
            //_connectionString = _connectionStringService.GetConnectionString("default");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.

            //    //optionsBuilder.UseSqlServer("Data Source=172.20.3.152\\MSSQL2017;Initial Catalog=NNGAccounts;Uid = sa; Password = aA@01737918236;");
            //    optionsBuilder.UseSqlServer(_connectionString);
            //}
            var tenant = _httpContextAccessor.HttpContext?.Items["Tenant"] as Tenant;

            if (tenant != null && !string.IsNullOrEmpty(tenant.ConnectionString))
            {
                optionsBuilder.UseSqlServer(tenant.ConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            return SaveChanges(acceptAllChangesOnSuccess: true);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var now = DateTime.UtcNow;
            ApplyAuditInfo(now);
            
            try
            {
                return base.SaveChanges(acceptAllChangesOnSuccess);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while saving changes to the database");
                throw;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            ApplyAuditInfo(now);
            
            try
            {
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while saving changes to the database asynchronously");
                throw;
            }
        }

        private void ApplyAuditInfo(DateTime now)
        {
            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is not Base baseEntity)
                    continue;

                var entry = Entry(baseEntity);

                switch (changedEntity.State)
                {
                    case EntityState.Added:
                        SetPropertyIfMapped(entry, nameof(Base.EntryDate), now);
                        SetPropertyIfMapped(entry, nameof(Base.IsActive), true);
                        break;

                    case EntityState.Modified:
                        SetPropertyIfModified(entry, nameof(Base.EntryBy), false);
                        SetPropertyIfModified(entry, nameof(Base.EntryDate), false);
                        SetPropertyIfMapped(entry, nameof(Base.UpdateDate), now);
                        break;
                }
            }
        }

        private void SetPropertyIfMapped(EntityEntry<Base> entry, string propertyName, object? value)
        {
            if (IsPropertyMapped(entry, propertyName))
            {
                entry.Property(propertyName).CurrentValue = value;
            }
        }

        private void SetPropertyIfModified(EntityEntry<Base> entry, string propertyName, bool isModified)
        {
            if (IsPropertyMapped(entry, propertyName))
            {
                entry.Property(propertyName).IsModified = isModified;
            }
        }

        private bool IsPropertyMapped(EntityEntry entry, string propertyName)
        {
            if (!_propertyCache.ContainsKey(propertyName))
            {
                _propertyCache[propertyName] = entry.Metadata.FindProperty(propertyName);
            }

            return _propertyCache[propertyName] != null;
        }

        private Tenant? GetCurrentTenant()
        {
            try
            {
                return _httpContextAccessor.HttpContext?.Items["Tenant"] as Tenant;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to retrieve tenant from HttpContext");
                return null;
            }
        }
    }
}
