using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class InvoiceDetailsWithItemInfo : Base
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
        public decimal? Discount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? Redeemed_PointValue { get; set; }
        public decimal? GiftVoucherDis { get; set; }
        public string? StyleNo { get; set; }
        public string? Barcode { get; set; }
        public string? VatType { get; set; }
        public string? ProductType { get; set; }
        public string? Category { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }

        // NEW fields from company and customer join
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? LogoPath { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? Cmail { get; set; }
        public string? SsmNubmer { get; set; }
        public string? TaxNumber { get; set; }
        public string? Unit { get; set; }
        public string? FooterInfo { get; set; }
        public string? CompCode { get; set; }         
        public bool? ComIsActive { get; set; }          
        public DateTime? ComEntryDate { get; set; }     
        public string? ComEntryBy { get; set; }         
        public string? ShortName { get; set; }          
        public string? ContactNo { get; set; }          
        public string? LabelPath { get; set; }          
        public string? ReceiptPath { get; set; }       
        public string? CompanyType { get; set; }
        public string? BarCodePath { get; set; }
        public string? BRN { get; set; }          
        public string? TIN { get; set; }       
        public string? AccountNo { get; set; }
        public string? Currency { get; set; }
        public string? CurrencyInfo { get; set; }
        public string? CurrencyDetails { get; set; }
    }
}
