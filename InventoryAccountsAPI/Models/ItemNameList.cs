using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class ItemNameList
    {
        public int? Id { get; set; }
        public int? ItemId { get; set; }
        public int VariantId { get; set; }
        public string? ReceiveNo { get; set; }
        public decimal CostingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string? StyleNo { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? BarCode { get; set; }
        public decimal RemainingQty { get; set; }
        public decimal Qty { get; set; }
        public decimal StockQty { get; set; }
        public decimal OldSellingPrice { get; set; }

    }
}
