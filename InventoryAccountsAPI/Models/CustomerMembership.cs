using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class CustomerMembership : Base
    {
        public string Name { get; set; }
        public string AccessKey { get; set; }
        public decimal Rule1 { get; set; }
        public decimal Rule2 { get; set; }
        public int Discount { get; set; }
    }
}
