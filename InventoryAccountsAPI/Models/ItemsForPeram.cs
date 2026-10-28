using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class ItemsForPeram :Base
    {
        public int ItemId { get; set; }
        public int VariantId { get; set; }
        public decimal? Qty { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public string? Code { get; set; }
        public Decimal? DiscountPercentage { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? PointValue { get; set; }
        public decimal? GiftVoucherDis { get; set; }


    }
}
