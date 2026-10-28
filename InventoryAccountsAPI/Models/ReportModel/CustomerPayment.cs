using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class CustomerPayment : Base
    {
        public string? CustomerMobile { get; set; }
        public int? InvoiceId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? PaidAmount { get; set; }
        //public string? PaymentType { get; set; }
        public int AccountId { get; set; }
    }
}
