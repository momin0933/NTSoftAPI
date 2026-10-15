using System.ComponentModel.DataAnnotations.Schema;

namespace BMSAPI.Models.Common
{
    public class POSItemReturnDetails :Base
    {
        public int? ReturnId { get; set; }
        [ForeignKey("ReturnId")]
        public virtual POSItemReturn? POSItemReturn { get; set; }
        public int ItemId { get; set; }
        public DateTime? InvoiceDt { get; set; }
        public string? InvoiceNo { get; set; }
        public int? Qty { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Discount { get; set; }
        public decimal? UnitPrice { get; set; }
       
    }
}
