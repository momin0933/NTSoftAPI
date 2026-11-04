namespace MerchantAPI.Models.Inspections
{
    public class InspectionReport : Base
    {
        public string? InspCompany { get; set; }
        public string? InspTypeShort { get; set; }
        public string? InspType { get; set; }
        public string? InspRef { get; set; }
        public string? ProductName { get; set; }
        public DateTime? InspDate { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public string? LogoPath { get; set; }
        public string? CoverPath { get; set; }
        public string? InspectorPhotoPath { get; set; }
        public string? SignaturePath { get; set; }
        public string? PORef { get; set; }
        public string? Buyer { get; set; }
        public string? EntityResponsible { get; set; }
        public string? ProductionSite { get; set; }
        public string? SiteName { get; set; }
        public string? SiteAddress { get; set; }
        public string? SitePhone { get; set; }
        public string? SiteRep { get; set; }
        public string? CoordSite { get; set; }
        public string? CoordStart { get; set; }
        public string? InspectorName { get; set; }
        public string? InspectorFrom { get; set; }
        public decimal? QtyOrdered { get; set; }
        public decimal? QtyProduced { get; set; }
        public decimal? ExpProducedPct { get; set; }
        public decimal? QtyPacked { get; set; }
        public decimal? ExpPackedPct { get; set; }
        public string? SKU { get; set; }
        public string? Description { get; set; }
        public string? SamplingMethod { get; set; }
        public string? SamplingLevel { get; set; }
        public int? LotSize { get; set; }
        public int? SampleSize { get; set; }
        public decimal? AqlCritical { get; set; }
        public decimal? AqlMajor { get; set; }
        public decimal? AqlMinor { get; set; }
        public int? AcceptCritical { get; set; }
        public int? AcceptMajor { get; set; }
        public int? AcceptMinor { get; set; }
        public int? DefectivePieces { get; set; }
        public int? CartonsAvailable { get; set; }
        public int? CartonsInspected { get; set; }
        public string? PcsPerCarton { get; set; }
        public string? PcsPerPolybag { get; set; }
        public string? MeasSize { get; set; }
        public string? MeasurementResult { get; set; }
        public string? OverrideResult { get; set; }
        public string? WorkmanshipResult { get; set; }
        public string? TestsResult { get; set; }
        public string? OverallResult { get; set; }
        public int? TotCritical { get; set; }
        public int? TotMajor { get; set; }
        public int? TotMinor { get; set; }
        public int? MeasOutCount { get; set; }
        public int? MeasReadingCount { get; set; }
        public string? SummaryReview { get; set; }
        public string? IssuedBy { get; set; }
        public string? ReviewedBy { get; set; }
        public DateTime? SignDate { get; set; }

        // child rows (sent and received with the report)
        public List<InspectionRemark> RemarkList { get; set; } = new();
        public List<InspectionDefect> Defects { get; set; } = new();
        public List<InspectionCheck> Checks { get; set; } = new();
        public List<InspectionCarton> Cartons { get; set; } = new();
        public List<InspectionMeasure> Measures { get; set; } = new();
        public List<InspectionPhoto> Photos { get; set; } = new();          // General / Start / Measurement
        public List<InspectionAttachment> Attachments { get; set; } = new();
    }
}
