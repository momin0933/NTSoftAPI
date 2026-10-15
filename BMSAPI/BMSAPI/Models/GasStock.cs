namespace BMSAPI.Models
{
    public class GasStock : Base
    {
        public int? BuildingId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Year { get; set; }
        public string? Month { get; set; }
        public decimal? OpeningQty { get; set; }
        public decimal? RcvQty { get; set; }
        public decimal? UsageQty { get; set; }
        public decimal? Stock { get; set; }
        public decimal? WastageQty { get; set; }
        public decimal? BuildingWisePrice { get; set; }

        public decimal? FinalPrice { get; set; }
    }
    public class GasStockCal:GasStock 
    {
        public decimal? TotalValue { get; set; }
        
    }
}
