namespace BMSAPI.Models.Common
{
    public class ItemTransfer :Base
    {
        //public string? CompanyName { get; set; }
        public int WhId { get; set; }       
        public int DestWhId { get; set; }
        public DateTime? TransferDt { get; set; }
        public string? TransferNo { get; set; }
        public decimal? TotalQty { get; set; }
        public string? Status { get; set; }
        public virtual ICollection<ItemTransferDetails>? ItemTransferList { get; set; }        
    }
}
