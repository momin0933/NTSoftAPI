using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class AccGLedger : Base
    {
        public int? CompCode { get; set; }
        public string? Code { get; set; }
        public string? Title { get; set; }
        public string? AccType { get; set; }

    }
}
