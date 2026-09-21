namespace BMSAPI.Models.Apps.PropHUB
{
    public class SendConnectionRequestBody
    {
        public int TenantUId { get; set; }
        public int TenantHomeId { get; set; }
        public string? LandlordMobile { get; set; }
        public int? TenancyId { get; set; }
        public int? PropertyId { get; set; }
        public int? PropDetailsId { get; set; }
        public string? MatchType { get; set; }
        public int? LandlordUId { get; set; }
        public string? EntryBy { get; set; }
    }
}
