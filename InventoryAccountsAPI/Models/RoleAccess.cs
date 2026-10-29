using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class RoleAccess
    {
        #region Public Properties  
        public string? Role { get; set; }
        public int MenuId { get; set; }
        public bool Access { get; set; }
        public int AutoId { get; set; }
        public int CompanyId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        //public string ModifiedBy { get; set; }
        //public DateTime? ModifiedDate { get; set; }
        #endregion
    }
}
