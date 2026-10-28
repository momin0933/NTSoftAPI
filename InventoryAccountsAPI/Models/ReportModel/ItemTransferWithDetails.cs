using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class ItemTransferWithDetails:Base
    {
        public int? ItemId { get; set; }
        public int? VariantId { get; set; }
        public string? ItemName { get; set; }
        public string? Barcode { get; set; }
        public string? Category { get; set; }
        public string? ProductType { get; set; }

        public string? Brand { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? TotalCostingPrice { get; set; }
        public decimal? TotalSellingPrice { get; set; }

        public int? FromWareHouse { get; set; }
        public int? ToWareHouse { get; set; }
        public DateTime? TransferDt { get; set; }
        public string? TransferNo { get; set; }
        public string? Status { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? Qty { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? StyleNo { get; set; }
        public int? TransferId { get; set; }
    }
}
