namespace BMSAPI.Models.Accounts
{
    public class RptLedger
    {
        public int GLId { get; set; }
        public string? AccType { get; set; }
        public string? GroupLedger { get; set; }
        public int? LedgerId { get; set; }
        public string? Ledger { get; set; }
        public decimal? Opening { get; set; }
        public decimal? DebitOpening { get; set; }
        public decimal? CreditOpening { get; set; }
        public string? TranType { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? CreditClosing { get; set; }
        public decimal? DebitClosing { get; set; }
        public string? CompanyName { get; set; }
        public string? ParentGroup { get; set; }
        public string? CompanyAddress { get; set; }

    }
}
