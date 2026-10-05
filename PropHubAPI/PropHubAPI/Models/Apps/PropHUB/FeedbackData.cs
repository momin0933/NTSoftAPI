namespace PropHubAPI.Models.Apps.PropHUB
{
    public class FeedbackData:Base
    {
        public int? UId { get; set; }
        public string? Phone { get; set; }
        public string? UserRole { get; set; }
        public int? Rating { get; set; }
        public string? Category { get; set; }
        public string? Message { get; set; }
        public string? Email { get; set; }
        public string? Platform { get; set; }
        public string? AppVersion { get; set; }
    }
}
