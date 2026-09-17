namespace BMSAPI.Models.Apps.PropHUB
{
    public class AddComplainRequest
    {
        public int UId { get; set; }
        public int TenantId { get; set; }
        public int? PropertyId { get; set; }
        public int? PropDetailsId { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public string? EntryBy { get; set; }
    }
}
