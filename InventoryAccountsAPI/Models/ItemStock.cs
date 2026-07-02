
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
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
