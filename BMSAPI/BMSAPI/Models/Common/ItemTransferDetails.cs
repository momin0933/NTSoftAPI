using System.ComponentModel.DataAnnotations.Schema;

namespace BMSAPI.Models.Common
{
    public class ItemTransferDetails :Base
    {
        public int TransferId { get; set; }
        [ForeignKey("TransferId")]
        public virtual ItemTransfer? ItemTransfer { get; set; }
        //[ForeignKey("TransferId")]
        //public virtual ItemTransfer Transfer { get; set; }
        // public int TransferId { get; set; }
        public int VariantId { get; set; }
        public int ItemId { get; set; }
        public decimal? Qty { get; set; }
        public decimal? RcvQty { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? Status { get; set; }


    }
}
