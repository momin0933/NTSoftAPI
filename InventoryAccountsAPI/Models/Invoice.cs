
using InventoryAccountsAPI.Models.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class Invoice: Base
    {
        public int WhId { get; set; }
        public DateTime? InvoiceDt { get; set; }
        public string? InvoiceNo { get; set; }
        public string? PaymentType { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? Discount { get; set; }
        public string? CustomerNumber { get; set; }
        public decimal Total { get; set; }
        public decimal? EarningPoint { get; set; }
        public decimal? RedeemedPoint { get; set; }
        public decimal? Vat { get; set; }
        public decimal? GiftVoucherDis { get; set; }
        public int? GiftVoucherId { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? SalesMan { get; set; }
        public string? Note { get; set; }
        public List<InvoiceDetailsWithItemInfo>? InvoiceDetails { get; set; }


    }
}
