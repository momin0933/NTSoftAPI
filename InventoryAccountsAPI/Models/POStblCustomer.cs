using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class POStblCustomer:Base
    {
        public string? Name { get; set; }
        public string? CusCode { get; set; }
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        public decimal? Reward_Points { get; set; }
        public int? MemberTypeId { get; set; }
        public decimal? Earn_Points { get; set; }
        public decimal? Redeemed_Points { get; set; }
        public string? MembershipType { get; set; }
        public int? Discount { get; set; }
        public DateTime? DOB { get; set; }            
        public string? Cmail { get; set; }            
        public string? SsmNubmer { get; set; }        
        public string? TaxNumber { get; set; }
        public string? AccountNo { get; set; }   
        public string? TIN { get; set; }   
        public string? BRN { get; set; }

        public virtual AccILedger? Ledger { get; set; }
    }
}
