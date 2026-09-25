namespace BMSAPI.Models.Apps.PropHUB
{
    public class UpdateExpenseNameRequest
    {
        public int Id { get; set; }
        public int UId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? EntryBy { get; set; }
    }
}
