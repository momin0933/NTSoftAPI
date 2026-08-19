namespace BMSAPI.Models.Apps.PropHUB
{
    public class TenantFullView
    {
        public int Id { get; set; }
        public string? Phone { get; set; }
        public int? PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyAddress { get; set; }
        public int? PropDetailsId { get; set; }
        public string? FlatName { get; set; }
        public string? Floor { get; set; }
        public string? TenantName { get; set; }
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
        public string? EName { get; set; }
        public string? EPhone { get; set; }
        public string? ERelation { get; set; }
        public string? EAddress { get; set; }
    }
}
