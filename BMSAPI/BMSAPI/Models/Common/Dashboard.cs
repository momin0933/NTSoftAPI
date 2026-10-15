namespace BMSAPI.Models.Common
{
    public class Dashboard
    {
        public int whId { get; set; }
       // public DateTime Date { get; set; }
        public decimal? TotalSale { get; set; }
        public decimal? TotalSalesAllTime { get; set; }
        public decimal? TotalProfit { get; set; }
        public int? NumberOfInvoice { get; set; }
        public int? TotalNumberOfInvoice { get; set; }
        public decimal? RcvCostingPrice { get; set; }
        public int RcvQty { get; set; }
        public decimal? TotalCostingPrice { get; set; }


    }

    public class TopSalesProduct
    {
        public string? StyleNo { get; set; }
        public string? Category { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }

    public class YearWiseProfit
    {
        public int Year { get; set; }
        public string Month { get; set; }
        public decimal MonthlyProfit { get; set; }
    }
}
