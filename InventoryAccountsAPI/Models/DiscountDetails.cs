
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class DiscountDetails : Base
    {
        public int DiscountId { get; set; }
        public int WhId { get; set; }
       // public int ItemId { get; set; }
        public int VariantId { get; set; }
    }
}
