namespace BMSAPI.Models
{
    public class AccVoucher:Base
    {
        public string? VoucherType { get; set; }
        public string? VoucherNumber { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string? PaymentType { get; set; }
        public string? Narration { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? VoucherStatus { get; set; }
        public int? CompanyId { get; set; }
        public int? UnitId { get; set; }
        public int? TeamId { get; set; }

        public virtual ICollection<VoucherDetails>? voucherEntryDetails { get; set; }
    }
}
