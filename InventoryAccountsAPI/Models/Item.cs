using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class Item : Base
    {
       // public string? Name { get; set; }
        //public string? Code { get; set; }
       // public string? BarCode { get; set; }
       // public string? SKU { get; set; }
        //public string? Color { get; set; }
        //public string? Size { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        //public decimal? CostingPrice { get; set; }
        //public decimal? SellingPrice { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Discount { get; set; }
        public string? StyleNo { get; set; }
        public string? ProductType { get; set; }
        public string? ImgPath { get; set; }
       // public bool? IsStockAlert { get; set; }
        public int? IsStockAlertQty { get; set; }
        public string? Status { get; set; }
        public string? EcomThumbnailUrls { get; set; }
        public string? EcomPreviewUrls { get; set; }
        public string? EcomDescription { get; set; }
        public string? EcomSpecification { get; set; }
        public string? PMTitle { get; set; }
        public string? PMDescription { get; set; }
        public bool? IsWebsite { get; set; }
        public virtual List<ItemStock>? ItemStockList { get; set; }
        public virtual List<ItemVariant>? ItemsList { get; set; }

    }
}
