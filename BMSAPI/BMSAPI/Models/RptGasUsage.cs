namespace BMSAPI.Models
{
    public class RptGasUsage
    {
        public int? Id { get; set; }
        public string? Year { get; set; }
        public string? Month { get; set; }
        public DateTime? Date { get; set; }
        public int? BuildingId { get; set; }
        public string? BuildingName { get; set; }
        public int? FlatOwnerId { get; set; }
        public int? FlatId { get; set; }
        public string? FlatOwnerName { get; set; }
        public string? FlatNumber { get; set; }
        public string? FlatCode { get; set; }
        public decimal? PreviousReading { get; set; }
        public decimal? CurrentReading { get; set; }
        public decimal? ConsumedCM { get; set; }
        public decimal? ConvertRate { get; set; }
        public decimal? TotalKg { get; set; }
        public int? IsAlreadyBilled { get; set; }
    }
}
