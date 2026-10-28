using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class POStblSalesMan:Base
    {     
        public string? Name { get; set; }
        public string? PhoneNo { get; set; }
        public string? Smail { get; set; }
        public string? AccountNo { get; set; }
        public string? Address { get; set; }
    }
}
