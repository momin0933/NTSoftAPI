namespace BMSAPI.Models.Apps.PropHUB
{
    public class AutoMatchResult
    {
        public int TenancyId { get; set; }
        public int PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public int LandlordUId { get; set; }
        public int PropDetailsId { get; set; }
        public string? FlatName { get; set; }
        public string? Floor { get; set; }
    }
}
