namespace BMSAPI.Models
{
    public class AccILedger : Base
    {
        public string? CompCode { get; set; }
        public int GLedgerId { get; set; }
        public string? LdgAccNo { get; set; }
        public string? Code { get; set; }
        public string? Title { get; set; }
        public int? ParentId { get; set; }
        public string? PaymentType { get; set; }         
        public string? ChequeNo { get; set; }         
        public string? BankNameBranch { get; set; }         
        public DateTime? ChequeDt { get; set; }
        public int? OPBalance { get; set; }

    }
}
