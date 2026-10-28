using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
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
