

namespace InventoryAccountsAPI.Models
{
    public class ItemVariant : Base
    {
        public int? ItemId { get; set; }
        public string? Barcode { get; set; }
        public string? SKU { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? Unit { get; set; }
        public string? VatType { get; set; }
        public decimal? Vat { get; set; }
        public string? VatCalculationType { get; set; }
        public virtual List<ItemStock>? ItemStockList { get; set; }

    }

    public class PriceUpdateRequest
    {
        public string ReceiveNo { get; set; }  // New ReceiveNo Parameter
        public string EntryBy { get; set; }
        public List<ItemVariant> Items { get; set; }
    }
}
