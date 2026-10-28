using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BMSAPI.BusinessLayer.Manager
{
    public class AcctblVoucherManager : IAcctblVoucher
    {
        private readonly NTSoftDbContextFactory _dbFactory;
        private readonly IDapperService _dapper;

        public AcctblVoucherManager(NTSoftDbContextFactory dbFactory, IDapperService dapper)
        {
            _dbFactory = dbFactory;
            _dapper = dapper;
        }

        // ---------------- Read ----------------

        public List<AccVoucher> GetAll()
        {
            using var db = _dbFactory.CreateDbContext();
            return db.voucherEntryDetails.AsNoTracking().ToList();
        }

        public AccVoucher? Get(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            return db.voucherEntryDetails.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public async Task<AccVoucher?> GetById(int id)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.voucherEntryDetails.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public RptAccVoucher GetVocherById(int id)
        {
            using var db = _dbFactory.CreateDbContext();

            var accVoucher = db.voucherEntryDetails.AsNoTracking().FirstOrDefault(x => x.Id == id)
                ?? throw new Exception($"Voucher not found. Id = {id}");

            // শুধু এই voucher-এর details আনা হয়, পুরো table না
            var details = _dapper.GetAllByQuery<RptVoucherDetails>(
                @"SELECT Il.Title AS LedgerName, v.*
                  FROM AcctblVoucherDetails AS v
                  JOIN AcctblILedger AS Il ON v.LedgerId = Il.Id
                  WHERE v.AccVoucherId = @id",
                new { id }).ToList();

            return new RptAccVoucher
            {
                Id = accVoucher.Id,
                VoucherNumber = accVoucher.VoucherNumber,
                Narration = accVoucher.Narration,
                CompanyId = accVoucher.CompanyId,
                UnitId = accVoucher.UnitId,
                TeamId = accVoucher.TeamId,
                IsActive = accVoucher.IsActive,
                EntryBy = accVoucher.EntryBy,
                PaymentType = accVoucher.PaymentType,
                TotalAmount = accVoucher.TotalAmount,
                VoucherType = accVoucher.VoucherType,
                VoucherStatus = accVoucher.VoucherStatus,
                VoucherDate = accVoucher.VoucherDate,
                rptVoucherDetails = details
            };
        }

        public List<RptAccVoucherListWithDetails> ACCGetAllVoucherListWithDetails(
            DateTime fromDate, DateTime toDate, string? vhoucherType, string? voucherNumber, int companyId)
        {
            var p = new DynamicParameters();
            p.Add("@fromDate", fromDate);
            p.Add("@toDate", toDate);
            p.Add("@voucherType", vhoucherType);
            p.Add("@voucherNumber", voucherNumber);

            if (companyId > 0)
            {
                p.Add("@companyId", companyId);
                p.Add("@QueryChecker", 1);
            }
            else
            {
                p.Add("@QueryChecker", 2);
            }

            return _dapper.GetAllBySP<RptAccVoucherListWithDetails>("SP_ACC_GetAllVoucherListWithDetails", p).ToList();
        }

        public List<string> GetVoucherNoByVoucherType(DateTime fromDate, DateTime toDate, string voucherType)
        {
            using var db = _dbFactory.CreateDbContext();

            // toDate দিনের পুরোটা ধরা হয় (রাত ১১:৫৯ পর্যন্ত)
            var end = toDate.Date.AddDays(1);

            var query = db.voucherEntryDetails.AsNoTracking()
                .Where(x => x.VoucherDate >= fromDate && x.VoucherDate < end);

            if (voucherType != "All")
                query = query.Where(x => x.VoucherType == voucherType);

            return query.Select(x => x.VoucherNumber!).ToList();
        }

        // ---------------- Add ----------------

        public int Add(AccVoucher entity)
        {
            EnsureBalanced(entity.voucherEntryDetails);

            using var db = _dbFactory.CreateDbContext();

            // voucher আর তার details একসাথে, এক SaveChanges-এ
            db.voucherEntryDetails.Add(entity);
            db.SaveChanges();

            return entity.Id;
        }

        // ---------------- Update ----------------

        public int Update(AccVoucher entity)
        {
            return UpdateOrAdd(entity);
        }

        public int UpdateOrAdd(AccVoucher voucher)
        {
            if (voucher.Id <= 0)
                return Add(voucher);

            var details = voucher.voucherEntryDetails?.ToList() ?? new List<VoucherDetails>();
            EnsureBalanced(details);

            using var db = _dbFactory.CreateDbContext();
            using var tx = db.Database.BeginTransaction();

            try
            {
                var existing = db.voucherEntryDetails.AsNoTracking().FirstOrDefault(x => x.Id == voucher.Id)
                    ?? throw new Exception($"Voucher not found. Id = {voucher.Id}");

                var existingDetails = db.VoucherEntry.AsNoTracking()
                    .Where(d => d.AccVoucherId == voucher.Id)
                    .ToList();

                // ১) পরিবর্তনের আগের অবস্থা log-এ রাখা
                db.AccVoucherLogs.Add(CreateVoucherLog(existing, "Edit", voucher.UpdateBy));
                foreach (var d in existingDetails)
                    db.AccVoucherDetailsLogs.Add(CreateDetailLog(d, "Edit", voucher.UpdateBy));

                // ২) Frontend IsActive না পাঠালে আগের মান রাখা, যাতে voucher লুকিয়ে না যায়
                voucher.IsActive ??= existing.IsActive;
                foreach (var d in details)
                {
                    d.AccVoucherId = voucher.Id;
                    d.IsActive ??= true;
                }

                // ৩) যে details frontend থেকে বাদ দেওয়া হয়েছে, সেগুলো মুছে ফেলা
                var keepIds = details.Where(d => d.Id > 0).Select(d => d.Id).ToList();
                var toDelete = db.VoucherEntry
                    .Where(d => d.AccVoucherId == voucher.Id && !keepIds.Contains(d.Id))
                    .ToList();
                db.VoucherEntry.RemoveRange(toDelete);

                // ৪) voucher + details: পুরনোগুলো update, নতুনগুলো (Id = 0) insert
                db.voucherEntryDetails.Update(voucher);

                db.SaveChanges();
                tx.Commit();

                return voucher.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the voucher and its details.", ex);
            }
        }

        // ---------------- Remove ----------------

        public int Remove(AccVoucher entity)
        {
            using var db = _dbFactory.CreateDbContext();
            using var tx = db.Database.BeginTransaction();

            var voucher = db.voucherEntryDetails
                .Include(v => v.voucherEntryDetails)
                .FirstOrDefault(v => v.Id == entity.Id)
                ?? throw new Exception($"Voucher not found. Id = {entity.Id}");

            var details = voucher.voucherEntryDetails?.ToList() ?? new List<VoucherDetails>();

            // ১) মুছে ফেলার আগে log
            db.AccVoucherLogs.Add(CreateVoucherLog(voucher, "Delete", entity.EntryBy));
            foreach (var d in details)
                db.AccVoucherDetailsLogs.Add(CreateDetailLog(d, "Delete", entity.EntryBy));

            // ২) আগে details, তারপর voucher
            db.VoucherEntry.RemoveRange(details);
            db.voucherEntryDetails.Remove(voucher);

            db.SaveChanges();
            tx.Commit();

            return voucher.Id;
        }

        // ---------------- helpers ----------------

        // মোট Debit আর মোট Credit সমান না হলে voucher সেভ হবে না
        private static void EnsureBalanced(IEnumerable<VoucherDetails>? details)
        {
            if (details == null || !details.Any())
                return;

            var debit = details
                .Where(d => string.Equals(d.TranType, "Dr", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount ?? 0);
            var credit = details
                .Where(d => string.Equals(d.TranType, "Cr", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount ?? 0);

            if (debit != credit)
                throw new InvalidOperationException($"Voucher is not balanced: Debit {debit} ≠ Credit {credit}.");
        }

        private static AccVoucherLog CreateVoucherLog(AccVoucher v, string status, string? by) => new()
        {
            VoucherId = v.Id,
            VoucherType = v.VoucherType,
            VoucherNumber = v.VoucherNumber,
            VoucherDate = v.VoucherDate,
            VoucherStatus = v.VoucherStatus,
            PaymentType = v.PaymentType,
            Narration = v.Narration,
            TotalAmount = v.TotalAmount,
            CompanyId = v.CompanyId,
            UnitId = v.UnitId,
            TeamId = v.TeamId,
            Status = status,
            EntryBy = by,
            EntryDate = DateTime.Now
        };

        private static AccVoucherDetailsLog CreateDetailLog(VoucherDetails d, string status, string? by)
        {
            bool isDebit = string.Equals(d.TranType, "Dr", StringComparison.OrdinalIgnoreCase);

            return new AccVoucherDetailsLog
            {
                VoucherDetailsId = d.Id,
                AccVoucherId = d.AccVoucherId,
                LedgerId = d.LedgerId,
                ShortDesc = d.ShortDesc,
                Amount = d.Amount,
                TranType = d.TranType,
                PaymentType = d.PaymentType,
                ChequeNo = d.ChequeNo,
                BankNbranch = d.BankNbranch,
                ChequeDate = d.ChequeDate,
                DebitAmount = isDebit ? d.Amount : 0,
                CreditAmount = isDebit ? 0 : d.Amount,
                Status = status,
                EntryBy = by,
                EntryDate = DateTime.Now
            };
        }
    }
}