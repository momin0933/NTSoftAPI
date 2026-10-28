using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptItemsForSales
    {
        public int? ItemId { get; set; }
        public int VariantId { get; set; }
        public int? WhId { get; set; }
       // public string? Name { get; set; }
        public string? Barcode { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? Discount { get; set;}
        public decimal? StockQty { get; set;}
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Code { get; set; }
        public string? StyleNo { get; set; }
        public string? ProductType { get; set; }
        public DateTime? EntryDate { get; set; }
        public decimal? Vat { get; set; }
        public string? DiscountName { get; set; }
        public string? Type { get; set; }
        public string? Unit { get; set; }
        public string? VatType { get; set; }
        public string? VatCalculationType { get; set; }
    }
}
