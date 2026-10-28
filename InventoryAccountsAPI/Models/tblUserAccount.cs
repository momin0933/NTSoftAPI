using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.Models
{
    public class tblUserAccount :Base
    {
        public int? UserAutoId { get; set; }
        public string? ClientName { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? UserId { get; set; }
        public string? Password { get; set; }
        public string? ReTypePassword { get; set; }
        public string? UserRole { get; set; }
        public DateTime? EntryDate { get; set; }
        public string? Supervisor { get; set; }
        public string? UserGroup { get; set; }
        public string? fldActive { get; set; }
        public int? EmployeeId { get; set; }
        public int? AccessControlID { get; set; }
        public string? Validator { get; set; }
        public string? NickName { get; set; }
        public string? Merchandiser { get; set; }
        public string? Email { get; set; }
        public string? ProfilePicture { get; set; }
        public string? MobileNo { get; set; }
        public string? PBXNo { get; set; }
        public string? Address { get; set; }
        public int? CompId { get; set; }
    }
}
