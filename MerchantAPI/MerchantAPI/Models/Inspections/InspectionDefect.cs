namespace MerchantAPI.Models.Inspections
{
    public class InspectionDefect : Base
    {
        public int? ReportId { get; set; }
        public int? SortOrder { get; set; }
        public string? Name { get; set; }
        public int? Critical { get; set; }
        public int? Major { get; set; }
        public int? Minor { get; set; }

        public List<InspectionPhoto> Photos { get; set; } = new();
    }
}
