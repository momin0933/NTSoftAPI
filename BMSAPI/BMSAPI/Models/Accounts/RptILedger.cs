namespace BMSAPI.Models
{
    public class RptILedger
    {
        public int? Id { get; set; }
        public int GLedgerId { get; set; }
        public string? LdgAccNo { get; set; }
        public string? Code { get; set; }
        public string? Title { get; set; }        
        public int? ParentId { get; set; }
        public string? GLCode { get; set;}
        public string? GLTitle { get; set; }
        public string? AccType { get; set; }
        public int? OPBalance { get; set; }
        public string? PaymentType { get; set; }
        public string? CompCode { get; set; }
    }
}
