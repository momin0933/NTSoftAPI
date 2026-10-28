using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class InvoiceDetail : Base
    {
        public int? InvoiceId { get; set; }
        public int ItemId { get; set; }
        public int VariantId { get; set; }
        public DateTime? InvoiceDt { get; set; }
        public string? InvoiceNo { get; set; }
        public decimal? Qty { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? Vat { get; set; }
        public  decimal? Discount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? Redeemed_PointValue { get; set; }
        public decimal? GiftVoucherDis { get; set; }
    }
}
