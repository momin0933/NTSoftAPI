namespace PropHubAPI.Models.Apps.PropHUB
{
    public class FeedbackView
    {
        public int Id { get; set; }
        public int? UId { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public string? UserRole { get; set; }
        public int Rating { get; set; }
        public string? Category { get; set; }
        public string? Message { get; set; }
        public string? Email { get; set; }
        public string? Platform { get; set; }
        public string? AppVersion { get; set; }
        public string? Status { get; set; }
        public string? AdminNote { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
