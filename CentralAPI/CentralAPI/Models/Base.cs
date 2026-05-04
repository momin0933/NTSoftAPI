using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralAPI.Models
{
    public class Base
    {      
        public int Id { get; set; }
        public string? Remarks { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? UpdateDate { get; set; } 
        public string? EntryBy { get; set; }
        public string? UpdateBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
