using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class InvoiceDropdown
    {
        public int Id { get; set; }               
        public string? InvoiceNo { get; set; }    
        public string? CustomerNumber { get; set; } 
        public string? CustomerName { get; set; }
    }
}
