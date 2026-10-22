namespace BMSAPI.Models.Common
{
    public class PriceLogDetails:Base
    {
        public int? LogId { get; set; }
        public int? ItemId { get; set; }
        public int? VariantId { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? NewPrice { get; set; }
    }
}
