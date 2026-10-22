namespace BMSAPI.Models
{
    public class RptGas
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public int? Qty { get; set; }
        public string? Unit { get; set; }
        public string? InvAttachment { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? BuildingId { get; set; }
        public string? BuildingName { get; set; }
        public string? SupplierName { get; set; }
    }
}
