namespace BMSAPI.Models.Apps.PropHUB
{
    public class TenantData:Base
    {
        public string? Phone { get; set; }
        public int? PropertyId { get; set; }
        public int? PropDetailsId { get; set; }
        public string? NID { get; set; }
        public string? TenantPhone { get; set; }
        public string? TenantEmail { get; set; }
        public DateTime? DOB { get; set; }
        public string? TenantType { get; set; }
        public string? Religion { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Advance { get; set; }
        public decimal? MonthlyAmount { get; set; }
        public string? PoliceForm { get; set; }
        public string? AgreementForm { get; set; }
        public string? EName { get; set; }
        public string? EPhone { get; set; }
        public string? ERelation { get; set; }
        public string? EAddress { get; set; }
    }
}
