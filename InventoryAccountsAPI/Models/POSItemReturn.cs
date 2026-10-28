using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class POSItemReturn :Base
    {
        public int InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }
        public string?  ReturnNo{ get; set; }
        public  DateTime? ReturnDt{ get; set; }
        public virtual ICollection<POSItemReturnDetails>? POSItemReturnDetails { get; set; }
    }
}
