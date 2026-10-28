using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class POStblItemIssue : Base
    {
        public int ItemId { get; set; }
        public int WhId { get; set; }
        public DateTime? IssueDt { get; set; }
        public string? IssueType { get; set; }
        public int? Qty { get; set; }
    }
}
