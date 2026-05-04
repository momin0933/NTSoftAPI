using Microsoft.EntityFrameworkCore;
using HRAPI.BusinessLayer.TenantService;
using HRAPI.Models;

namespace HRAPI.BusinessLayer.Service
{
    public class NTSoftDbContext : DbContext
    {
        public NTSoftDbContext(DbContextOptions<NTSoftDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tenant> Tenants { get; set; } = null!;
        public DbSet<Order> Orders { get; set; }  // ✅ এই লাইন যোগ করুন

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>().ToTable("tblTenant");
            modelBuilder.Entity<Order>().ToTable("tblOrders");
        }

        public override int SaveChanges()
        {
            var now = DateTime.Now;

            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is Base entity)
                {
                    var entry = Entry(entity);

                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            if (IsPropertyMapped(entry, nameof(Base.EntryDate)))
                                entity.EntryDate = now;

                            if (IsPropertyMapped(entry, nameof(Base.IsActive)))
                                entity.IsActive = true;
                            break;

                        case EntityState.Modified:
                            if (IsPropertyMapped(entry, nameof(Base.EntryBy)))
                                entry.Property(x => x.EntryBy).IsModified = false;

                            if (IsPropertyMapped(entry, nameof(Base.EntryDate)))
                                entry.Property(x => x.EntryDate).IsModified = false;

                            if (IsPropertyMapped(entry, nameof(Base.UpdateDate)))
                                entity.UpdateDate = now;

                            if (IsPropertyMapped(entry, nameof(Base.IsActive)))
                                entry.Property(x => x.IsActive).IsModified = true;
                            break;
                    }
                }
            }

            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;

            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is Base entity)
                {
                    var entry = Entry(entity);

                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            if (IsPropertyMapped(entry, nameof(Base.EntryDate)))
                                entity.EntryDate = now;

                            if (IsPropertyMapped(entry, nameof(Base.IsActive)))
                                entity.IsActive = true;
                            break;

                        case EntityState.Modified:
                            if (IsPropertyMapped(entry, nameof(Base.EntryBy)))
                                entry.Property(x => x.EntryBy).IsModified = false;

                            if (IsPropertyMapped(entry, nameof(Base.EntryDate)))
                                entry.Property(x => x.EntryDate).IsModified = false;

                            if (IsPropertyMapped(entry, nameof(Base.UpdateDate)))
                                entity.UpdateDate = now;
                            break;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private bool IsPropertyMapped(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, string propertyName)
        {
            var prop = entry.Metadata.FindProperty(propertyName);
            return prop != null;
        }
    }
}