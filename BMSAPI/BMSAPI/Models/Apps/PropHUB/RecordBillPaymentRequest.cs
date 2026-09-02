namespace BMSAPI.Models.Apps.PropHUB
{
    public class RecordBillPaymentRequest
    {
        public int BillId { get; set; }
        public string? Phone { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string? PaymentType { get; set; }
        public string? Remarks { get; set; }
        public string? EntryBy { get; set; }
    }
}
