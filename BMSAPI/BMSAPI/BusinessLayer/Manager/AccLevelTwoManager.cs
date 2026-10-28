using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using BMSAPI.Models.Accounts;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BMSAPI.BusinessLayer.Manager
{
    public class AccLevelTwoManager : IAccLevelTwo
    {
        private readonly NTSoftDbContextFactory _dbFactory;
        private readonly IDapperService _dapper;

        public AccLevelTwoManager(NTSoftDbContextFactory dbFactory, IDapperService dapper)
        {
            _dbFactory = dbFactory;
            _dapper = dapper;
        }

        // ---------------- CRUD ----------------

        public List<AccILedger> GetAll()
        {
            using var db = _dbFactory.CreateDbContext();
            return db.AccLevelTwo.AsNoTracking().Where(x => x.IsActive == true).ToList();
        }

        public AccILedger? Get(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            return db.AccLevelTwo.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public async Task<AccILedger?> GetById(int id)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.AccLevelTwo.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public int Add(AccILedger entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.AccLevelTwo.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int Update(AccILedger entity)
        {
            using var db = _dbFactory.CreateDbContext();
            db.AccLevelTwo.Update(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public int Remove(AccILedger entity)
        {
            using var db = _dbFactory.CreateDbContext();

            var ledger = db.AccLevelTwo.FirstOrDefault(x => x.Id == entity.Id)
                ?? throw new Exception($"Ledger not found. Id = {entity.Id}");

            // Soft delete: ledger-এ voucher থাকতে পারে, তাই row না মুছে নিষ্ক্রিয় করা হয়
            ledger.IsActive = false;
            ledger.UpdateBy = entity.UpdateBy;
            db.SaveChanges();

            return ledger.Id;
        }

        // ---------------- Chart of accounts ----------------

        public List<RptILedger> GetAllGledgerWithIlegder()
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 1);
            return _dapper.GetAllBySP<RptILedger>("SP_AccCOA", p).ToList();
        }

        public List<BaseDropDown> GetAllLedgerForDropdown(DateTime fromDate, DateTime toDate)
        {
            var p = new DynamicParameters();
            p.Add("@fromDate", fromDate);
            p.Add("@toDate", toDate);
            p.Add("@querySelect", 1);
            p.Add("@ledgerId", null);
            return _dapper.GetAllBySP<BaseDropDown>("SP_ACC_GetLedgerReportDetails", p).ToList();
        }

        // ---------------- Ledger reports ----------------

        public List<RptLedgerReport> GetLedgerListWithVoucherDetails(DateTime fromDate, DateTime toDate, int? ledgerId, int? companyId)
        {
            var p = new DynamicParameters();
            p.Add("@fromDate", fromDate);
            p.Add("@toDate", toDate);
            p.Add("@ledgerId", ledgerId);
            p.Add("@companyId", companyId);
            p.Add("@querySelect", LedgerQuerySelect(ledgerId, companyId));
            return _dapper.GetAllBySP<RptLedgerReport>("SP_ACC_GetLedgerReportDetails", p).ToList();
        }

        public List<RptLedgerReport> GetLedgerListWithBalance(DateTime fromDate, DateTime toDate, int? ledgerId, int? companyId, int? supplierId)
        {
            var p = new DynamicParameters();
            p.Add("@fromDate", fromDate);
            p.Add("@toDate", toDate);
            p.Add("@ledgerId", ledgerId);
            p.Add("@companyId", companyId);
            p.Add("@supplierId", supplierId);
            p.Add("@querySelect", LedgerQuerySelect(ledgerId, companyId));
            return _dapper.GetAllBySP<RptLedgerReport>("SP_ACC_GetLedgerWithBalance", p).ToList();
        }

        // 2 = নির্দিষ্ট ledger + নির্দিষ্ট company
        // 4 = নির্দিষ্ট ledger + সব company
        // 3 = সব ledger
        private static int LedgerQuerySelect(int? ledgerId, int? companyId)
        {
            var ledger = ledgerId ?? 0;
            var company = companyId ?? 0;

            if (ledger != 0 && company > 0) return 2;
            if (ledger != 0 && company == 0) return 4;
            return 3;
        }

        public List<RptLedger> GetLedgerWithTrialBalance(DateTime fromDate, DateTime toDate, int companyId)
        {
            var p = new DynamicParameters();
            p.Add("@fromDate", fromDate);
            p.Add("@toDate", toDate);
            p.Add("@companyId", companyId);
            p.Add("@QueryChecker", companyId > 0 ? 1 : 2);
            return _dapper.GetAllBySP<RptLedger>("SP_AccTrialBalance", p).ToList();
        }

        // ---------------- Financial statements ----------------

        public List<RptProfitLoss> IncomeStatement(DateTime fromDate, DateTime toDate, int companyId)
        {
            var p = new DynamicParameters();
            p.Add("@fromDate", fromDate);
            p.Add("@toDate", toDate);
            p.Add("@companyId", companyId);
            p.Add("@QueryChecker", companyId > 0 ? 5 : 4);
            return _dapper.GetAllBySP<RptProfitLoss>("Sp_AccIncomeStatement", p).ToList();
        }

        public List<RptProfitLoss> BalanceSheet(DateTime fromDate, DateTime toDate, int companyId)
        {
            var p = new DynamicParameters();
            p.Add("@fromDate", fromDate);
            p.Add("@toDate", toDate);
            p.Add("@companyId", companyId);
            p.Add("@QueryChecker", companyId > 0 ? 2 : 3);
            return _dapper.GetAllBySP<RptProfitLoss>("Sp_AccIncomeStatement", p).ToList();
        }

        public List<RptReceiptPayment> ReceiptNPayment(DateTime fromDate, DateTime toDate, int companyId)
        {
            var p = new DynamicParameters();
            p.Add("@fromDate", fromDate);
            p.Add("@toDate", toDate);
            p.Add("@companyid", companyId);
            p.Add("@QueryChecker", companyId > 0 ? 1 : 2);
            return _dapper.GetAllBySP<RptReceiptPayment>("Sp_AccReceipt_Payment", p).ToList();
        }
    }
}