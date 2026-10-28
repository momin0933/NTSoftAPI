using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class AccLevelThree : Base
    {
        public int LevelTwoAutoID { get; set; }
        public string? LevelThreeCode { get; set; }
        public string? LevelThreeDescription { get; set; }

    }
}
