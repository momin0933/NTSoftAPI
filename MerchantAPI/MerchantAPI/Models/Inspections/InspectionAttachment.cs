namespace MerchantAPI.Models.Inspections
{
    public class InspectionAttachment : Base
    {
        public int? ReportId { get; set; }
        public int? SortOrder { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
    }
}
