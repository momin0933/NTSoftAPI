using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class POStblAccount:Base
    {
        public string? AccountType { get; set; }
        public string? AccountName { get; set; }
        public string? AccCode { get; set; }
        public string? BankName { get; set; }
        public string? AccountNo { get; set; }
    }
}
