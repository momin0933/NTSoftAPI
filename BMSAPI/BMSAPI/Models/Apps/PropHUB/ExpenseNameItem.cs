namespace BMSAPI.Models.Apps.PropHUB
{
    public class ExpenseNameItem
    {
        public int Id { get; set; }
        public int? UId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsCustom { get; set; }
    }
}
