namespace BMSAPI.Models.Common
{
    public class ItemTransferReceive:Base
    {
        public int WhId { get; set; }
        public int DestWhId { get; set; }
        public DateTime? TransferDt { get; set; }
        public string? TransferNo { get; set; }
        public int? TotalQty { get; set; }
        public List<ItemTransferDetailList> lists { get; set; }
 
    }
    public class ItemTransferDetailList : Base
    {
        public int TransferId { get; set; }
        public int ItemId { get; set; }
        public int? Qty { get; set; }
        public string? Status { get; set; }


    }
}
