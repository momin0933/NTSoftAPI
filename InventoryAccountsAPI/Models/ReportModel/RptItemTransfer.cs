
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptItemTransfer
    {
        public string? CompanyName { get; set; }
        public int WhId { get; set; }
        public int DestWhId { get; set; }
        public DateTime? TransferDt { get; set; }
        public string? TransferNo { get; set; }
        public decimal? TotalQty { get; set; }
        public string? Status { get; set; }
        public virtual ICollection<ItemTransferDetails>? ItemTransferList { get; set; }
    }
}
