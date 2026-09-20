namespace BMSAPI.Models.Apps.PropHUB
{
    public class ConnectionRequestItem
    {
        public int Id { get; set; }
        public int TenantUId { get; set; }
        public int TenantHomeId { get; set; }
        public string? LandlordMobile { get; set; }
        public int? TenancyId { get; set; }
        public int? PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public int? PropDetailsId { get; set; }
        public string? FlatName { get; set; }
        public string? Floor { get; set; }
        public string? MatchType { get; set; }
        public string? Status { get; set; }
        public DateTime? RequestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string? TenantEnteredPropertyName { get; set; }
        public string? TenantEnteredUnitNo { get; set; }
        public string? TenantName { get; set; }
        public string? TenantPhone { get; set; }
    }
}
