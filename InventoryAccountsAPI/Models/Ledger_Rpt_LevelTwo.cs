using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class Ledger_Rpt_LevelTwo
    {


        public int Id { get; set; }
        public string? LevelOneAutoID { get; set; }
        public string? LevelTwoDescription { get; set; }
    }
}
