namespace BMSAPI.Models.Apps.PropHUB
{
    public class UpdateBillPaymentRequest
    {
        public int BillId { get; set; }
        public string? Phone { get; set; }
        public decimal? PaidAmount { get; set; }
    }
}
