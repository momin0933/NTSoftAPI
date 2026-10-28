using BMSAPI.Models;
using BMSAPI.Models.Accounts;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IAccLevelTwo : ICommonMethod<AccILedger>
    {
        //public List<AccLevelTwo> Dropdown();
        public Task<AccILedger> GetById(int Lvel2AutoID);
        public List<RptILedger> GetAllGledgerWithIlegder();
        public List<BaseDropDown> GetAllLedgerForDropdown(DateTime fromDate, DateTime toDate);
        public List<RptLedgerReport> GetLedgerListWithVoucherDetails(DateTime fromDate, DateTime toDate, int? ledgerId, int? companyId);
        public List<RptLedgerReport> GetLedgerListWithBalance(DateTime fromDate, DateTime toDate, int? ledgerId, int? companyId, int? supplierId);

        public List<RptLedger> GetLedgerWithTrialBalance(DateTime fromDate, DateTime toDate, int companyId);
        public List<RptProfitLoss> IncomeStatement(DateTime fromDate, DateTime toDate, int companyId);


        // public List<RptProfitLoss> GetIncomeStatement(DateTime fromDate, DateTime toDate);
        public List<RptProfitLoss> BalanceSheet(DateTime fromDate, DateTime toDate, int companyId);


        public List<RptReceiptPayment> ReceiptNPayment(DateTime fromDate, DateTime toDate, int companyId);





    }
}
