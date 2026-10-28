using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class Ledger_LevelThreeReport
    {
        public int Id { get; set; }
        public int LevelTwoAutoId { get; set; }
        public string? LevelThreeCode { get; set; } 
        
    }
}
