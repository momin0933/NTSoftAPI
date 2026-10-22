namespace BMSAPI.Models
{
    public class Expenses : Base
    {
        public string? Year { get; set; }
        public string? Month { get; set; }
        public string? Name { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string? ExpenseType { get; set; }
        public int? BuildingId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}