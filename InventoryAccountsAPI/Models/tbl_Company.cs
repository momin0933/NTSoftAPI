using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class tbl_Company
    {
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? EntryDate { get; set; }
        public int? EntryById { get; set; }
        public string? ShortName { get; set; }
        public string? ContactNo { get; set;}
        public string? LogoPath { get; set; }
        public string? LabelPath { get; set; }
        public string? ReceiptPath { get; set; }

        public string? CompanyType { get; set; }

    }
}
