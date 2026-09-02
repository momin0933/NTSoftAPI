using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IBill
    {
        GenerateBillResult GenerateBill(string phone, string billMonth, string billYear, string entryBy);
        IEnumerable<BillView> GetBillList(string phone, string billMonth, string billYear);
        bool CheckBillExists(string phone, string billMonth, string billYear);
        RecordBillPaymentResult RecordBillPayment(int billId, string phone, decimal paymentAmount, string paymentType, string remarks, string entryBy);
        IEnumerable<BillPaymentView> GetBillPaymentHistory(string phone, int billId);
    }
}
