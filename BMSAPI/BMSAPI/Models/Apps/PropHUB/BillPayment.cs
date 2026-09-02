namespace BMSAPI.Models.Apps.PropHUB
{
    public class BillPayment :Base
    {
        public string? Phone { get; set; }
        public int? BillId { get; set; }
        public int? PropertyId { get; set; }
        public int? PropDetailsId { get; set; }
        public int? TenantId { get; set; }
        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentType { get; set; }

    }
}
