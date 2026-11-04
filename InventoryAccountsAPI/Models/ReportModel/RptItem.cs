using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class RptItem
    {
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? VariantId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? BarCode { get; set; }
        public string? SKU { get; set; }
        public string? Color { get; set; }
        public string? Category { get; set; }
        public string? Size { get; set; }
        public string? EcomThumbnailUrls { get; set; }
        public string? Brand { get; set; }
        public string? Description { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ReceiveQty { get; set; }
        public decimal? ActualReceiveQty { get; set; }
        public decimal? IssueQty { get; set; }
        public decimal? SellQty { get; set; }
        public decimal? StockQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public decimal? TransferQty { get; set; }
        public decimal? PhysicalAdjQty { get; set; }
        public int? IsStockAlertQty { get; set; }
        public string? StyleNo { get; set; }
        public string? ImgPath { get; set; }
        public string? ProductType { get; set; }
        public DateTime? EntryDate { get; set; }
        public string? Remarks { get; set; }
        public string? Unit { get; set; }
        public string? VatType { get; set; }
        public string? SourceType { get; set; }
        public string? ProductionType { get; set; }
        public string? VatCalculationType { get; set; }
        public decimal? TotalCostingPrice { get; set; }
        public decimal? TotalSellingPrice { get; set; }

        // ===== Business profile fields (Electronics / Furniture) — SP_ItemStock QueryChecker = 4 থেকে =====
        public string? WarrantyType { get; set; }
        public int? WarrantyMonths { get; set; }
        public bool? IsSerialRequired { get; set; }
        public string? Specification { get; set; }
    }
}
