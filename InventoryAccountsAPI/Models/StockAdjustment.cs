using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class StockAdjustment:Base
    {
        public int WhId { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        
    }

    public class StockAdjustmentModel : Base
    {
        public int AdjustmentId { get; set; }
        public int ItemId { get; set; }
        public int VariantId { get; set; }
        public string Barcode { get; set; }
        public int Qty { get; set; }

    }

    public class FinalStockAdjustment : Base
    {
        public int AdjustmentId { get; set; }
        public int ItemId { get; set; }
        public int VariantId { get; set; }
        public string Barcode { get; set; }
        public int PhyQty { get; set; }
        public int StockQty { get; set; }
        public int AdjustmentQty { get; set; }
        public string Comment { get; set; }
        public int WhId { get; set; }
    }
}
