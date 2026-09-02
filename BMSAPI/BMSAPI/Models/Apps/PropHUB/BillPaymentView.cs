namespace BMSAPI.Models.Apps.PropHUB
{
    public class BillPaymentView
    {
        public int Id { get; set; }
        public int? BillId { get; set; }
        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentType { get; set; }
        public string? Remarks { get; set; }
        public string? EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
    }
}
