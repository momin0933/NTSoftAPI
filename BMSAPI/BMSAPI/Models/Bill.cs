namespace BMSAPI.Models
{
    public class Bill:Base
    {
        public DateTime? Date { get; set; }
        public string? Year { get; set; }
        public string? Month { get; set; }
        public DateTime? CollectionDate { get; set; }
        public string? FlatCode { get; set; }
        public decimal? BillAmount { get; set; }
        public decimal? Collection { get; set; }
        public string? Status { get; set; }
    }
}
