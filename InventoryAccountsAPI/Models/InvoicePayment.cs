using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class InvoicePayment
    {
        public int AccountId { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
    }
}
