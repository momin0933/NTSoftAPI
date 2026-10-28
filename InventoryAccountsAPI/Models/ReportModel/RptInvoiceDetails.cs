using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptInvoiceDetails
    {
        public int? ItemId { get; set; }
        public int? VariantId { get; set; }
        public string? Name { get; set; }
        public decimal? Qty { get; set; }
        public string? Unit { get; set; }

        public string? PaymentType { get; set; }
        public decimal? Vat { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? InvoiceDt { get; set; }
        public decimal? Discount { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? CostingPrice { get; set; }

        public string? InvoiceNo { get; set; }
        public string? BarCode { get; set; }
        public string? StyleNo { get; set;}
        public string? ProductType { get; set; }
        public string? Category { get; set;}
        public string? Color { get; set;}
        public string? Size { get; set; }
        public decimal? Total { get; set; }
        public decimal? GrandTotal { get; set; }
        public decimal? Redeemed_PointValue { get; set; }
        public decimal? GiftVoucherDis { get; set; }
        public string? VatType { get; set; }
        public string? VatCalculationType { get; set; }
        public string? CustomerNumber { get; set; }
        public string? LedgerId { get; set; }
        public string? CustomerName { get; set; }
    }
}
