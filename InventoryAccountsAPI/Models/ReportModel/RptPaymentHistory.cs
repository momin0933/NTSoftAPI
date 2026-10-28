using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class RptPaymentHistory
    {
        public int CustomerId { get; set; }              
        public string? CustomerName { get; set; }        
        public DateTime PaymentDate { get; set; }
        public string? InvoiceNo { get; set; }
        public string? CustomerMobile { get; set; }       
        public decimal PaidAmount { get; set; }           
        public string? PaymentMethod { get; set; }
    }
}
