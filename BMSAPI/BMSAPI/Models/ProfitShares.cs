using System.ComponentModel.DataAnnotations.Schema;

namespace BMSAPI.Models
{
    public class ProfitShares : Base
    {
        public string? Period { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? Income { get; set; }
        public decimal? Expenses { get; set; }
        public decimal? Profit { get; set; }
        public decimal? PSPercent { get; set; }
        public decimal? ProfitShare { get; set; }

        public List<ProfitShareDetails>? ProfitShareDetails { get; set; }
    }
    public class ProfitShareDetails : Base
    {
        public int EmployeeId { get; set; }

        public decimal Percentage { get; set; }
        public decimal Value { get; set; }
        public bool? IsDisbursement { get; set; }
        public DateTime? DisDate { get; set; }

        // Foreign key to ProfitShare
        public int ProfitShareId { get; set; }

        [ForeignKey("ProfitShareId")]
        public ProfitShares? ProfitShare { get; set; }
    }

}
