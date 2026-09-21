namespace BMSAPI.Models.Apps.PropHUB
{
    public class DeactivateHomeRequest
    {
        public int TenantHomeId { get; set; }
        public int UId { get; set; }
        public string? EntryBy { get; set; }
    }
}
