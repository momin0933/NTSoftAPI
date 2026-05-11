namespace BMSAPI.Models
{
    public class ViewModalBill:Base
    {
        public int CompanyId { get; set; }
        public string? FlatCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? BillNo { get; set; }
        public List<Bill>? Bills { get; set; }
    }
}
