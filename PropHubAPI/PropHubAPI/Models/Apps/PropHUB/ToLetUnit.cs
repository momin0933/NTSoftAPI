namespace PropHubAPI.Models.Apps.PropHUB
{
    public class ToLetUnit
    {
        public int PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public string? Address { get; set; }
        public string? PropertyType { get; set; }
        public int PropDetailsId { get; set; }
        public string? FlatName { get; set; }
        public string? Floor { get; set; }
        public int? Room { get; set; }
        public int? Bathroom { get; set; }
        public int? Balcony { get; set; }
        public DateTime? AvailableSince { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerPhone { get; set; }
        public string? SecurityName { get; set; }
        public string? SecurityPhone { get; set; }
    }
}
