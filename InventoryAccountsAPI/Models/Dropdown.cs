using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class dropdown
    {
        public int Id { get; set; } 
        public string Name { get; set; }
    }
    public class Project
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
    }

    public class ReceiveNoList
    {
        public int Id { get; set; }
        public string? ReceiveNo { get; set; }
    }
}
