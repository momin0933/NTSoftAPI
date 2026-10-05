namespace PropHubAPI.Models.Apps.PropHUB
{
    public class UpdateFeedbackStatusRequest
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? AdminNote { get; set; }
        public string? EntryBy { get; set; }
    }
}
