using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class UserRoles : Base
    {       
        public string UserRole { get; set; }       
        public string? ProjectName { get; set; }
        public int CompId { get; set; }
    }
}
