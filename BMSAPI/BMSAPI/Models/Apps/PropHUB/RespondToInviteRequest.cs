namespace BMSAPI.Models.Apps.PropHUB
{
    public class RespondToInviteRequest
    {
        public int TenantId { get; set; }
        public int TenantUserId { get; set; }
        public bool Accept { get; set; }
        public string? EntryBy { get; set; }
    }
}
