using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class ItemsForDropDown
    {
        public int? id { get; set; }
        public string? StyleNo { get; set; }
        public string? StyleAndDescription { get; set; }
    }
}
