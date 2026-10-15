namespace BMSAPI.Models
{
    public class Gas : Base
    {
        public DateTime? Date { get; set; }
        public int? Qty { get; set; }
        public string? Unit { get; set; }
        public string? InvAttachment { get; set; }
        public decimal?  CostingPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? SupplierId { get; set; }
        public string? Year { get; set; }
        public string? Month { get; set; }
        public int? BuildingId { get; set; }
    }
}
