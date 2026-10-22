namespace BMSAPI.Models
{
    public class AccVoucherDetailsLog : Base
    {
        public int VoucherDetailsId { get; set; }
        public int AccVoucherId { get; set; }
        public string? Status { get; set; }
        public int? LedgerId { get; set; }
        public string? ShortDesc { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public string? TranType { get; set; }
        public string? PaymentType { get; set; }
        public string? ChequeNo { get; set; }
        public string? BankNbranch { get; set; }
        public DateTime? ChequeDate { get; set; }

    }
}
