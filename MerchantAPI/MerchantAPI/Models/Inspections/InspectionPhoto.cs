namespace MerchantAPI.Models.Inspections
{
    public class InspectionPhoto : Base
    {
        public int? ReportId { get; set; }
        public string? Section { get; set; }      // Defect / Check / General / Start / Measurement
        public int? DefectId { get; set; }
        public int? CheckId { get; set; }
        public int? SortOrder { get; set; }
        public string? FilePath { get; set; }
        public string? Caption { get; set; }
    }
}
