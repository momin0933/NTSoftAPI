using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class ItemVariantUpdate
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Brand { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? ItemCode { get; set; }
        public string? Remarks { get; set; }
        public string? Description { get; set; }
        public string? BarCode { get; set; }
        public decimal? Vat { get; set; }
        public string? VatType { get; set; }
        public string? Unit { get; set; }
        public string? VatCalculationType { get; set; }
        public string? SourceType { get; set; }
        public string? ProductionType { get; set; }
        public decimal? Discount { get; set; }
        public string? ProductType { get; set; }
        public string? StyleNo { get; set; }

        public int ItemId { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }


        public string? ImgPath { get; set; }
        public string? PMTitle { get; set; }
        public string? PMDescription { get; set; }
        public int? IsStockAlertQty { get; set; }
        public string? EcomThumbnailUrls { get; set; }
        public string? EcomPreviewUrls { get; set; }
        public string? EcomDescription { get; set; }
        public string? EcomSpecification { get; set; }
        public bool? IsWebsite { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
    }
}
