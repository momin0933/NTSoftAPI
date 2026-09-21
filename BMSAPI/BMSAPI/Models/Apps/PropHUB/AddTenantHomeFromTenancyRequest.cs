namespace BMSAPI.Models.Apps.PropHUB
{
    public class AddTenantHomeFromTenancyRequest
    {
        public int UId { get; set; }
        public int SourceTenantId { get; set; }
        public string? EntryBy { get; set; }
    }
}
