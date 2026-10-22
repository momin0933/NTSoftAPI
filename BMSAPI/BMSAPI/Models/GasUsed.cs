using Microsoft.EntityFrameworkCore;

namespace BMSAPI.Models
{
    public class GasUsed : Base
    {
        public string? Year { get; set; }
        public string? Month { get; set; }
        public DateTime? Date { get; set; }
        public int? BuildingId { get; set; }
        public int? FlatId { get; set; }
        public int? OwnerId { get; set; }
    
        public string? FCode { get; set; }
        [Precision(10, 4)]
        public decimal? PreviousReading { get; set; }
        [Precision(10, 4)]
        public decimal? CurrentReading { get; set; }
        [Precision(10, 4)]
        public decimal? ConsumedCM { get; set; }
        public decimal? ConvertRate { get; set; }
        [Precision(10, 4)]
        public decimal? TotalKg { get; set; }
    }


}
