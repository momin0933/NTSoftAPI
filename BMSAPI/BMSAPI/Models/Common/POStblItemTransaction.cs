namespace BMSAPI.Models.Common
{
    public class POStblItemTransaction : Base
    {
        public int ItemId { get; set; }
        public int VariantId { get; set; }
        public int WhId { get; set; }
        public DateTime? TranDt { get; set;}
        public decimal? Qty { get; set; }
        public string? Status { get; set; }
    }
}
