namespace BMSAPI.Models.Apps.PropHUB
{
    public class ExpenseView
    {
        public int Id { get; set; }
        public string? Phone { get; set; }
        public string? ExpenseType { get; set; }
        public string? ExpenseName { get; set; }
        public int? PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public int? PropDetailsId { get; set; }
        public string? FlatName { get; set; }
        public string? Floor { get; set; }
        public int? TenantId { get; set; }
        public string? TenantName { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string? PaymentType { get; set; }
        public string? Remarks { get; set; }
        public string? EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
    }
}
