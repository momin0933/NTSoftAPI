using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace BMSAPI.Models.Common
{
    public class POStblItemRcvDetails : Base
    {
      
        public int RcvId { get; set; }
        [ForeignKey("RcvId")]
        public virtual ItemReceive? ItemReceive { get; set; }   
        public int ItemId { get; set; }
        public int VariantId { get; set; }
        public decimal? Qty { get; set; }
        public decimal? IssueQty { get; set; }
        public decimal? RemainingQty { get; set; }
        public string? Status { get; set; }
        public decimal? CostingPrice { get; set; }  
        public decimal? SellingPrice { get; set;}
    }
}
