using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptBrandWiseSalesReport
    {
        public string? Brand { get; set; }
        public int? Qty { get; set; }
        public decimal? SellingValue { get; set; }
        public string? Category { get; set; }
        public string? ProductType { get; set; }
        public string? SourceType { get; set; }
        public string? ProductionType { get; set; }
        public string? StyleNo { get; set; }
    }
}
