using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models.ReportModel
{
    public class RptModule
    {
        [Key]
        public int ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public string? ModuleIcon { get; set; }
        public string? ModuleLblName { get; set; }
        public string? ModuleLblIcon { get; set; }
        public bool IsActive { get; set; }
        public int ModuleSorting { get; set; }
    }

    public class ModuleDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool Checked { get; set; }
        // Add other properties from tblmvcmodule as needed
    }

}
