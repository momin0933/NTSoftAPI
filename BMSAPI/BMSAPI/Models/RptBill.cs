namespace BMSAPI.Models
{
    public class RptBill:Base
    {
        public DateTime? Date { get; set; }
        public string? Year { get; set; }
        public string? Month { get; set; }
        public string? BuildingName { get; set; }
        public string? FlatNumber { get; set; }
        public string? OwnerName { get; set; }
        public string? FlatCode { get; set; }
        public decimal? BillAmount { get; set; }
        public decimal? Collection { get; set; }
        public string? Status { get; set; }
        public int Billstatus { get; set; }
        public string? TntPhone { get; set; }
        public string? BillNo { get; set; }
        public string? TntEmail { get; set; }
    }
}
