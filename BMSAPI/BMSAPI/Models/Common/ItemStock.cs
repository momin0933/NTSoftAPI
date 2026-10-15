using System.ComponentModel.DataAnnotations.Schema;

namespace BMSAPI.Models.Common
{
    public class ItemStock :Base
    {
        #region Public Properties  
      
        public int ItemId { get; set; }
        public int WhId { get; set; }
        public decimal? ReceiveQty { get; set; }
        public decimal? IssueQty { get; set; }
        public decimal? StockQty { get; set; }
        public decimal? ReturnQty { get; set; }
        public int VariantId { get; set; }
        public decimal? PrevQty { get; set; }
        public decimal? AdjQty { get; set; }
        //// Navigation Property
        [ForeignKey("VariantId")]
        public virtual ItemVariant? ItemVariant { get; set; }
        //public virtual ICollection<POStblItemRcvDetails>? ItemRcvDetailsLists { get; set; }
        //public int RcvId { get; set; }
        #endregion
    }
}
