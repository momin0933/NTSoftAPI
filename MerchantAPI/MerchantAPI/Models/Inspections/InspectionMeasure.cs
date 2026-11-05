namespace MerchantAPI.Models.Inspections
{
    public class InspectionMeasure : Base
    {
        public int? ReportId { get; set; }
        public int? SortOrder { get; set; }
        public string? Name { get; set; }
        public string? TolMinus { get; set; }
        public string? TolPlus { get; set; }
        public string? Spec { get; set; }
        public string? Readings { get; set; }
        public int? OutCount { get; set; }
        public int? ReadingCount { get; set; }
    }
}
