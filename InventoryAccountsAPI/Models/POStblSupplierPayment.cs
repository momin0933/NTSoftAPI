using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class POStblSupplierPayment:Base
    {
        public int? SupplierId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? TotalPaid { get; set; }
        public string? VoucherNumber { get; set; }
        public List<POStblSupplierPaymentDetails> PaymentDetails { get; set; } = new();
    }
}
