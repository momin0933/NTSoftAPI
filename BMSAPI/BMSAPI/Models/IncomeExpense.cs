namespace BMSAPI.Models
{
    public class IncomeExpense:Base
    {
        public int ProductId { get; set; }
        public string? ExpensesName { get; set; }
        public DateTime ExpensesDate { get; set; }
        public string? ExpensesType { get; set; }
        public decimal Amount { get; set; }
        public string? EmployeeName { get; set; }
        public string? ClientName { get; set; }
        public string? Status { get; set; }
    }
}
