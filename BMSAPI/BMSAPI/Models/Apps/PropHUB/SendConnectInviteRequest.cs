namespace BMSAPI.Models.Apps.PropHUB
{
    public class SendConnectInviteRequest
    {
        public int UId { get; set; }
        public int TenantId { get; set; }
        public string? EntryBy { get; set; }
    }
}
