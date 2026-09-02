namespace BMSAPI.Models.Apps.PropHUB
{
    public class RecordBillPaymentResult
    {
        public int AffectedRows { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
    }
}
