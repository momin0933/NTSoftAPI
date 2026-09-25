namespace BMSAPI.Models.Apps.PropHUB
{
    public class AddExpenseNameRequest
    {
        public int UId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? EntryBy { get; set; }
    }
}
