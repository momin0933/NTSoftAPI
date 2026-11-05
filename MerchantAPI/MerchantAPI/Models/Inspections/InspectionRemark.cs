namespace MerchantAPI.Models.Inspections
{
    public class InspectionRemark : Base
    {
        public int? ReportId { get; set; }
        public int? SortOrder { get; set; }
        public string? Text { get; set; }
    }
}
