namespace BMSAPI.Models.Apps.PropHUB
{
    public class MarkComplainReadRequest
    {
        public int ComplainId { get; set; }
        public int UId { get; set; }
        public string? EntryBy { get; set; }
    }
}
