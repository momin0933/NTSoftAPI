using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IBkashManager
    {
        // GET operations (Dapper)
        BkashBillInfo GetBillMonthWise(string UserName, string Password,string FlatCode, string BillMonth);
        //bool SaveBkashPayment(BkashPaymentRequest request);
        public BkashBillPaymentResponse SaveBkashPayment(BkashPaymentRequest request);

        //BkashBillInfo GetBillByTrxId(string UserName, string Password, string TrxId);
        BkashBillPaymentResponse GetBillByTrxId(string UserName, string Password, string TrxId);

    }
}
