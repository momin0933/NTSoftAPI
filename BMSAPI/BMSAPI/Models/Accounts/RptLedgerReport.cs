namespace BMSAPI.Models
{
    public class RptLedgerReport
    {
        public DateTime? VoucherDate { get; set; }
        public string? opBalance { get; set; }
        public string? vNumber { get; set; }
        public decimal? opAmount { get; set; }
        public decimal? Amount { get; set; }
        public string? Ledger { get; set; }
        public string? TranType { get; set; }
        public string? NarrationHead { get; set; }
        public string? Narration { get; set; }
        public string? VoucherType { get; set; }
        public string? LedgerPeram { get; set; }
        public int? SortOrder { get; set; }
        public string? Balance { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }

    }
}
