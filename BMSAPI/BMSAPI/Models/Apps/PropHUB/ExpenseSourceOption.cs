namespace BMSAPI.Models.Apps.PropHUB
{
    public class ExpenseSourceOption
    {
        public int TenantId { get; set; }
        public string? TenantName { get; set; }
        public int PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public int PropDetailsId { get; set; }
        public string? FlatName { get; set; }
        public string? Floor { get; set; }
    }
}
