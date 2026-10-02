namespace PropHubAPI.Models.Apps.PropHUB
{
    public class MoveOutTenantRequest
    {
        public int TenantId { get; set; }
        public int UId { get; set; }
        public DateTime? MoveOutDate { get; set; }
        public string? EntryBy { get; set; }
    }
}
