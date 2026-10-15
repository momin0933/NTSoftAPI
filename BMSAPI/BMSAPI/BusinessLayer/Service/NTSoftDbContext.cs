using Microsoft.EntityFrameworkCore;
using BMSAPI.BusinessLayer.TenantService;
using BMSAPI.Models;
using BMSAPI.Models.Common;

namespace BMSAPI.BusinessLayer.Service
{
    public class NTSoftDbContext : DbContext
    {
        public NTSoftDbContext(DbContextOptions<NTSoftDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tenant> Tenants { get; set; } = null!;
        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<VbntblBkashPayments> VbntblBkashPayments { get; set; } = null!;
        public virtual DbSet<AccVoucher> voucherEntryDetails { get; set; } = null!;
        public virtual DbSet<VoucherDetails> VoucherEntry { get; set; } = null!;
        public virtual DbSet<AccGLedger> Accounts { get; set; } = null!;
        //BUILDING-MANAGER
        public virtual DbSet<Owner> Owners { get; set; } = null!;
        public virtual DbSet<FlatOwner> FlatOwners { get; set; } = null!;
        public virtual DbSet<Building> Buildings { get; set; } = null!;
        public virtual DbSet<Flats> Flats { get; set; } = null!;
        public virtual DbSet<Expenses> Expenses { get; set; } = null!;
        public virtual DbSet<ServiceCharge> ServiceCharges { get; set; } = null!;
        public virtual DbSet<VwFlatOwner> VWFlatOwners { get; set; } = null!;
        public virtual DbSet<Gas> Gases { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;
        public virtual DbSet<GasStock> GasStocks { get; set; } = null!;
        public virtual DbSet<GasUsed> GasUseds { get; set; } = null!;
        public virtual DbSet<ClosingDeclaration> ClosingDeclarations { get; set; } = null!;
        public virtual DbSet<CommitteeMember> CommitteeMembers { get; set; } = null!;
        public virtual DbSet<CommitteeSmSMail> CommitteeSmSMails { get; set; } = null!;
        public virtual DbSet<SmsMailWithMember> SmsMailWithMembers { get; set; } = null!;
        public virtual DbSet<CustomerSmsMail> CustomerSmSMails { get; set; } = null!;
        public virtual DbSet<SmsMailWithCustomer> SmsMailWithCustomers { get; set; } = null!;
        public virtual DbSet<AccILedger> AccLevelTwo { get; set; } = null!;
        public virtual DbSet<UserRoles> UserRoles { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>().ToTable("tblTenant");
            modelBuilder.Entity<VbntblBkashPayments>().ToTable("VbntblBkashPayments");
            modelBuilder.Entity<Bill>().ToTable("VbnTblBill");
            modelBuilder.Entity<AccVoucher>().ToTable("AcctblVoucher");
            modelBuilder.Entity<AccGLedger>().ToTable("AcctblGLedger");
            modelBuilder.Entity<VoucherDetails>().ToTable("AcctblVoucherDetails",
              tb => tb.UseSqlOutputClause(false));

            //BUILDING-MANAGER
            modelBuilder.Entity<Owner>().ToTable("vbntblOwners");
            modelBuilder.Entity<FlatOwner>().ToTable("vbntblFlatOwners");
            modelBuilder.Entity<Building>().ToTable("vbntblBuilding");
            modelBuilder.Entity<Flats>().ToTable("vbntblFlats");
            modelBuilder.Entity<Expenses>().ToTable("vbntblExpense");
            modelBuilder.Entity<ServiceCharge>().ToTable("vbntblServiceCharge");
            modelBuilder.Entity<VwFlatOwner>().ToView("Vw_FlatOwnerInfo");
            modelBuilder.Entity<Gas>().ToTable("vbntblGas");
            modelBuilder.Entity<Supplier>().ToTable("vbntblSupplier");
            modelBuilder.Entity<GasStock>().ToTable("vbntblGasStock");
            modelBuilder.Entity<GasUsed>().ToTable("vbntblGasUsed");
            modelBuilder.Entity<ClosingDeclaration>().ToTable("vbntblGasStockOpening");
            modelBuilder.Entity<CommitteeMember>().ToTable("vbntblCommitteeMembers");
            modelBuilder.Entity<CommitteeSmSMail>().ToTable("vbntblCommitteeSmsMail");
            modelBuilder.Entity<SmsMailWithMember>().ToTable("vbntblSmsMailWithMembers");
            modelBuilder.Entity<SmsMailWithCustomer>().ToTable("tblSmsMailCustomers");
            modelBuilder.Entity<UserRoles>().ToTable("tblUserRole");
            modelBuilder.Entity<CustomerSmsMail>().ToTable("tblSmsMail");
            modelBuilder.Entity<AccILedger>().ToTable("AcctblILedger");
            modelBuilder.Entity<CommitteeSmSMail>()
                .HasMany(c => c.Members)
                .WithOne(m => m.CommitteeSmSMail)
                .HasForeignKey(m => m.SMSMailId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SmsMailWithMember>()
                .Property(m => m.SMSMailId).HasColumnName("SMSMailId");

            modelBuilder.Entity<CustomerSmsMail>()
                .HasMany(c => c.Members)
                .WithOne(m => m.CustomerSmsMail)
                .HasForeignKey(m => m.SMSMailId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SmsMailWithCustomer>()
                .Property(m => m.SMSMailId).HasColumnName("SMSMailId");
            modelBuilder.Entity<SmsMailWithCustomer>()
                .Property(m => m.Phone).HasColumnName("Phone");
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