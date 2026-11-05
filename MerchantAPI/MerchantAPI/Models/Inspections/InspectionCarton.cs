namespace MerchantAPI.Models.Inspections
{
    public class InspectionCarton : Base
    {
        public int? ReportId { get; set; }
        public int? SortOrder { get; set; }
        public string? Sku { get; set; }
        public string? CartonNo { get; set; }
        public string? QtyPerCarton { get; set; }
        public string? Dimension { get; set; }
        public string? Weight { get; set; }
    }
}
