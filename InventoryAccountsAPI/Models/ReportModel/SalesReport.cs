using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class SalesReport
    {
        public decimal GrossSell { get; set; }
        public decimal Discount { get; set; }
        public decimal Point { get; set; }
        public decimal Voucher { get; set; }
        public decimal TotalCosting { get; set; }
        public decimal NetSales { get; set; }
        public decimal GrossProfit { get; set; }
    }

    public class SalesChartReport
    {
       
            public string SaleDate { get; set; } // Format: "yyyy-MM-dd"
            public decimal GrossSell { get; set; }
            public decimal NetSales { get; set; }
            public decimal Discount { get; set; }
           public decimal GrossProfit { get; set; }

    }
}
