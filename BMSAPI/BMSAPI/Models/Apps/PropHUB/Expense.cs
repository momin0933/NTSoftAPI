namespace BMSAPI.Models.Apps.PropHUB
{
    public class Expense:Base
    {
        public string? Phone { get; set; }
        public string? ExpenseType { get; set; }
        public string? ExpenseName { get; set; }
        public int? PropertyId { get; set; }
        public int? PropDetailsId { get; set; }
        public int? TenantId { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string? PaymentType { get; set; }
    }
}
