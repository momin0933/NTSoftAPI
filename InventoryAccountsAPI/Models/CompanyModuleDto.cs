using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class CompanyModuleDto
    {
        public int CompanyId { get; set; }
        public List<int> ModuleIds { get; set; }
    }
}
