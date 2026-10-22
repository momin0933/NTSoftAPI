namespace BMSAPI.Models.Common
{
    public  class ItemReceive : Base
    {
        #region Public Properties  
       
       
        public int WhId { get; set; }
        public DateTime? ReceiveDt { get; set; }
        public string? ReceiveNo { get; set; }
        public int? SupplierId { get; set; }
        public string? Supplier { get; set; }
        public decimal? TotalQty { get; set; }
        public string? Status { get; set; }
        public string? FilePath { get; set; }
        public virtual ICollection<POStblItemRcvDetails>? ItemRcvDetailsList { get; set; }

        #endregion
    }
  
}
