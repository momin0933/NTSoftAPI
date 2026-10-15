namespace BMSAPI.Models
{
    public class RptGasExpense
    {
        public decimal? PreviousReading { get; set; }
        public decimal? CurrentReading { get; set; }
        public decimal? ConvertRate { get; set; }
        public decimal? ConsumedCM { get; set; }
        public decimal? TotalKg { get; set; }
        public decimal? FinalPrice { get; set; }
    }

    public class RptExtraCharge
    {
        public string? Description { get; set; }
        public decimal ExCharge { get; set; }
    }
}
