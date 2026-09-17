namespace BMSAPI.Models.Apps.PropHUB
{
    public class UpdateComplainStatusRequest
    {
        public int ComplainId { get; set; }
        public int UId { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public string? EntryBy { get; set; }
    }
}
