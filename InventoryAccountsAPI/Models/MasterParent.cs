using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.Common
{
    public class MasterParent:Base
    {
        public string? ParentId { get; set; }
        public string? ParentName { get; set; }
   
    }
}
