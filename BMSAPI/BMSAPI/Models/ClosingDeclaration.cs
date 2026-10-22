namespace BMSAPI.Models
{
    public class ClosingDeclaration :Base
    {
        public int? BuildingId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? DueCollectionDate { get; set; }
        public string? Year { get; set; }
        public string? Month { get; set; }
        public decimal? OpeningQty { get; set; }
    }
}
