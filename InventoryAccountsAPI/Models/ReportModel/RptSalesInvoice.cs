using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptSalesInvoice
    {
        public int? Id { get; set; }
        public string? InvoiceNo { get; set; }
        public  DateTime? InvoiceDt { get; set; }
        public int? WhId { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Total { get; set; }
        public decimal? Discount { get; set; }
        public decimal? RedeemedPoint { get; set; }
        public decimal? GrandTotal { get; set; }
        public string? CompanyName { get; set; }
        public string? PaymentType { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? GiftVoucherDis { get; set; }
        public decimal CostingPrice { get; set; }
    }
}
