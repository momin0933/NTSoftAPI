using Microsoft.EntityFrameworkCore;
using CentralAPI.BusinessLayer.TenantService;
using CentralAPI.Models;

namespace CentralAPI.BusinessLayer.Service
{
    public class NTSoftDbContext : DbContext
    {
        public NTSoftDbContext(DbContextOptions<NTSoftDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tenant> Tenants { get; set; } = null!;
        public virtual DbSet<RefreshToken> RefreshToken { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>().ToTable("tblTenant");
            modelBuilder.Entity<RefreshToken>().ToTable("tblRefreshToken");
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

        private bool IsPropertyMapped(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, string propertyName)
        {
            var prop = entry.Metadata.FindProperty(propertyName);
            return prop != null;
        }
    }
}