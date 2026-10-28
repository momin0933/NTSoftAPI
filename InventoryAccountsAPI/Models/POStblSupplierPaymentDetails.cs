using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class POStblSupplierPaymentDetails:Base
    {
        [ForeignKey("SupplierPayment")]
        public int? PaymentId { get; set; }

        public int? InvoiceId { get; set; }

        public decimal? PaidAmount { get; set; }

        public POStblSupplierPayment? SupplierPayment { get; set; }
    }
}
