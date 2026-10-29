using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class ProcedureExecuteRequest
    {
        public string Operation { get; set; } // "READ", "INSERT", "UPDATE", "DELETE"
        public string ProcedureName { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
