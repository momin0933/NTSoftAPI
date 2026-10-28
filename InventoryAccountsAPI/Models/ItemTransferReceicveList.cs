using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class ItemTransferReceicveList
    {
        public int WhId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal CostingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string Status { get; set; }
    }
}
