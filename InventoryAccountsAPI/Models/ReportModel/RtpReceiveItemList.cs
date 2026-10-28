using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RtpReceiveItemList
    {
        //public string? Name { get; set; }
        public int? Id { get; set; }
        public decimal? Qty { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public int? RcvId { get; set; }
        public string? StyleNo { get; set; }

        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Category { get; set; }
        public string? ProductType { get; set; }
        public string? Barcode { get; set; }
        public decimal? TotalCostingPrice { get; set; }
        public decimal? TotalSellingPrice { get; set; }
    }
}
