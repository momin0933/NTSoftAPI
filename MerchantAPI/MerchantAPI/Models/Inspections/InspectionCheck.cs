namespace MerchantAPI.Models.Inspections
{
    public class InspectionCheck : Base
    {
        public int? ReportId { get; set; }
        public int? SortOrder { get; set; }
        public string? GroupName { get; set; }
        public string? CheckKey { get; set; }
        public string? CheckName { get; set; }
        public string? Result { get; set; }
        public string? SampleQty { get; set; }
        public string? Remark { get; set; }

        public List<InspectionPhoto> Photos { get; set; } = new();
    }
}
