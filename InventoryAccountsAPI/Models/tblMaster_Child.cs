using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class MasterChild:Base
    {
        public int ChildId { get; set; }
        public string? Value { get; set; }
        public string? ChildName { get; set; }
        public string? ParentId { get; set; }
    }

}
