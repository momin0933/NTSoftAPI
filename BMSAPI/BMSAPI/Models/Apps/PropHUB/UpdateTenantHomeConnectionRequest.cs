namespace BMSAPI.Models.Apps.PropHUB
{
    public class UpdateTenantHomeConnectionRequest
    {
        public int TenantHomeId { get; set; }
        public int UId { get; set; }
        public string? ConnectionStatus { get; set; }
        public int? PropertyId { get; set; }
        public int? PropDetailsId { get; set; }
        public string? EntryBy { get; set; }
    }
}
