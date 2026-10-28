using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class CustomerDropdown
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNo { get; set; }
        public string? LedgerId { get; set; }
    }
}
