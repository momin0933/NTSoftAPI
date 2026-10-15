namespace BMSAPI.Models.Common
{
    public class ItemRecievePerams :Base
    {
   
        public int ItemId { get; set; }
        public int WhId { get; set; }
        public string? ReceiveNo { get; set; }
        public DateTime? ReceiveDt { get; set; }
        public int? SuplierId { get; set; }
        public int? Qty { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal ItemSellingPrice { get; set; }
        public decimal ItemVat { get; set; }
        public decimal ItemDiscount { get; set; }
    }
}
