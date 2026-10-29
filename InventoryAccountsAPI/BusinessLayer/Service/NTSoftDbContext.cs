using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.TenantService;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.EntityFrameworkCore;

namespace InventoryAccountsAPI.BusinessLayer.Service
{
    public class NTSoftDbContext : DbContext
    {
        public NTSoftDbContext(DbContextOptions<NTSoftDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tenant> Tenants { get; set; } = null!;
        //public DbSet<IItem> Orders { get; set; }  
        public virtual DbSet<UserAccount> UserAccounts { get; set; } = null!;
        public virtual DbSet<ItemReceive> ItemReceives { get; set; } = null!;
        public virtual DbSet<POStblItemRcvDetails> POStblItemRcvDetails { get; set; } = null!;
        public virtual DbSet<Item> Items { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; } = null!;
        public virtual DbSet<AccLevelThree> AccLevelThree { get; set; } = null!;
        public virtual DbSet<ChartOfAccount> ChartOfAccount { get; set; } = null!;
        public virtual DbSet<VoucherDetails> VoucherEntry { get; set; } = null!;
        public virtual DbSet<AccVoucher> voucherEntryDetails { get; set; } = null!;
        public virtual DbSet<MasterChild> MasterChild { get; set; } = null!;
        //public virtual DbSet<tbl_Company> Tbl_Companies { get; set; } = null!;
        public virtual DbSet<POStblSupplier> POStblSuppliers { get; set; } = null!;
        public virtual DbSet<POStblCustomer> POStblCustomers { get; set; } = null!;
        //public virtual DbSet<tblUserAccount> TblUserAccounts { get; set; } = null!;
        public virtual DbSet<POStblItemTransaction> POStblItemTransactions { get; set; } = null!;
        public virtual DbSet<ItemTransfer> ItemTransfers { get; set; } = null!;
        public virtual DbSet<ItemTransferDetails> ItemTransferDetails { get; set; } = null!;
        public virtual DbSet<POStblItemIssue> POStblItemIssues { get; set; } = null!;
        public virtual DbSet<UserRoles> UserRoles { get; set; } = null!;
        public virtual DbSet<POSItemReturn> POSItemReturns { get; set; } = null!;
        public virtual DbSet<POSItemReturnDetails> POSItemReturnDetails { get; set; } = null!;
        public virtual DbSet<CompanyModuleMap> tblCompanyModuleMap { get; set; } = null!;
        public virtual DbSet<Discount> POStblDiscount { get; set; } = null!;
        public virtual DbSet<DiscountDetails> POStblDiscountDetails { get; set; } = null!;
        public virtual DbSet<InventoryAdj> InventoryAdjs { get; set; } = null!;
        public virtual DbSet<InventoryAdjDetails> InventoryAdjDetails { get; set; } = null!;
        public virtual DbSet<CustomerPayment> CustomerPayments { get; set; } = null!;

        public virtual DbSet<POStblCategory> Categories { get; set; } = null!;
        public virtual DbSet<POStblAccount> bankAccounts { get; set; } = null!;
        public virtual DbSet<POStblSalesMan> salasMan { get; set; } = null!;
        public virtual DbSet<POStblSupplierPayment> SupplierPayment { get; set; } = null!;
        public virtual DbSet<POStblSupplierPaymentDetails> SupplierPaymentDetails { get; set; } = null!;
        public virtual DbSet<GiftVoucher> GiftVouchers { get; set; } = null!;
        public virtual DbSet<GiftVoucherDetails> GiftVoucherDetails { get; set; } = null!;
        public virtual DbSet<CustomerMembership> CustomerMemberships { get; set; } = null!;

        public virtual DbSet<StockAdjustment> StockAdjustments { get; set; } = null!;
        public virtual DbSet<StockAdjustmentModel> StockAdjustmentModels { get; set; } = null!;
        public virtual DbSet<FinalStockAdjustment> FinalStockAdjustments { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>().ToTable("tblTenant");
            modelBuilder.Entity<IItem>().ToTable("POStblItem");
            modelBuilder.Entity<ItemReceive>().ToTable("POStblItemRcv");
            modelBuilder.Entity<POStblItemRcvDetails>().ToTable("POStblItemRcvDetails");
            modelBuilder.Entity<ItemStock>().ToTable("POStblItemStock");
            modelBuilder.Entity<Item>().ToTable("POStblItem");
            modelBuilder.Entity<ItemVariant>().ToTable("POStblItemVariant");
            modelBuilder.Entity<Invoice>().ToTable("POStblInvoice");
            modelBuilder.Entity<InvoiceDetail>().ToTable("POStblInvoiceDetails");
            modelBuilder.Entity<AccLevelThree>().ToTable("AcctblLevelThree");
            modelBuilder.Entity<ChartOfAccount>().ToTable("tbl_ChartOfAcc");
            modelBuilder.Entity<VoucherDetails>().ToTable("AcctblVoucherDetails");
            modelBuilder.Entity<AccVoucher>().ToTable("AcctblVoucher");
            modelBuilder.Entity<MasterChild>().ToTable("tblMaster_Child");
            modelBuilder.Entity<POStblSupplier>().ToTable("POStblSupplier");
            modelBuilder.Entity<POStblCustomer>().ToTable("POStblCustomer");
            modelBuilder.Entity<POStblItemTransaction>().ToTable("POStblItemTransaction");
            modelBuilder.Entity<ItemTransfer>().ToTable("POStblItemTransfer");
            modelBuilder.Entity<ItemTransferDetails>().ToTable("POStblItemTransferDetails");
            modelBuilder.Entity<POStblItemIssue>().ToTable("POStblItemIssue");
            modelBuilder.Entity<UserRoles>().ToTable("tblUserRole");
            modelBuilder.Entity<POSItemReturn>().ToTable("POStblReturn");
            modelBuilder.Entity<POSItemReturnDetails>().ToTable("POStblReturnDetails");
            modelBuilder.Entity<Discount>().ToTable("POStblDiscount");
            modelBuilder.Entity<DiscountDetails>().ToTable("POStblDiscountDetails");

            modelBuilder.Entity<InventoryAdj>().ToTable("PostblInventoryAdj");
            modelBuilder.Entity<InventoryAdjDetails>().ToTable("PostblInventoryAdjDetails");

            modelBuilder.Entity<CustomerPayment>().ToTable("POStblCustomerPayment");


            modelBuilder.Entity<POStblCategory>().ToTable("POStblCategory");
            modelBuilder.Entity<POStblAccount>().ToTable("POStblAccount");
            modelBuilder.Entity<POStblSalesMan>().ToTable("POStblSalesMan");
            modelBuilder.Entity<POStblSupplierPayment>().ToTable("POStblSupplierPayment");
            modelBuilder.Entity<POStblSupplierPaymentDetails>().ToTable("POStblSupplierPaymentDetails");
            modelBuilder.Entity<GiftVoucher>().ToTable("POStblGiftVoucher");
            modelBuilder.Entity<GiftVoucherDetails>().ToTable("POStblGiftVoucherDetails");
            modelBuilder.Entity<CustomerMembership>().ToTable("POStblCustomerMembership");

            modelBuilder.Entity<StockAdjustment>().ToTable("PosTblPhyAdj");
            modelBuilder.Entity<StockAdjustmentModel>().ToTable("PosTblPhyAdjDetails");
            modelBuilder.Entity<FinalStockAdjustment>().ToTable("PosTblPhyAdjMap");
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