using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class InventoryAdj : Base
    {
        public int WhId { get; set; }
        public string? AdjNo { get; set; }
        public string? Type { get; set; }
        public DateTime AdjDate { get; set; }
        public int TotalQty { get; set; }
        public List<InventoryAdjDetails>? AdjDetails { get; set; }
    }

    public class InventoryAdjDetails: Base
    {
        public int InventoryAdjId { get; set; }
        [ForeignKey("InventoryAdjId")]
        public InventoryAdj? InventoryAdj { get; set; }
        public int ItemId { get; set; }
        public int VariantId { get; set; }
        public DateTime AdjDate { get; set; }
        public int Qty { get; set; }
        public decimal CostingPrice { get; set; }
        public decimal SellingPrice { get; set; }
    }
}
