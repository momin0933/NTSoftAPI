using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class PaymentPerams
    {
        public int? InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentDate { get; set; }
        public string? EntryBy { get; set; }
        public int PaymentTypeId { get; set; }
        public int LedgerId { get; set; }
        public string Remarks { get; set; }
        public DateTime? EntryDate { get; set; }
    }

 
}
