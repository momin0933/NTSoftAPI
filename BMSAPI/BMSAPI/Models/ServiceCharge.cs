namespace BMSAPI.Models
{
    public class ServiceCharge : Base
    {
        public string? Year { get; set; }
        public string? Month { get; set; }
        public int? TotalFlat { get; set; }
        public int? TotalFlatUsingMosque { get; set; }
       
        public decimal? TotalExpense { get; set; }
        public decimal? FlatWiseExpense { get; set; }
        public decimal? TotalFlatUsingMosqueBill { get; set; }
        public decimal? FlatWiseMosqueBill { get; set; }
        public decimal? FlatWiseServiceCharge { get; set; }

    }
}
 