namespace BMSAPI.Models.Common
{
    public class Discount:Base
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual List<DiscountDetails>? DiscountDetails { get; set; }

    }
}
