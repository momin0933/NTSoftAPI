
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralAPI.Models
{
    public class UserAccount : Base
    {


        #region Public Properties  
        public int UserAutoId { get; set; }
        public string? Name { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? UserId { get; set; }
        public string? Password { get; set; }
        public string? ReTypePassword { get; set; }
        public string? UserRole { get; set; }
        public string? Supervisor { get; set; }
        public string? UserGroup { get; set; }
        public string? Unit { get; set; }
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
        public string? UserACAutoID { get; set; }
        public string? OrgCode { get; set; }
        public string? UserLavel { get; set; }
        public string? MaxAuthLevel { get; set; }
        public DateTime? DateFormate { get; set; }
        public string? FullName { get; set; }
        public string? ImageName { get; set; }
        public int? CompId { get; set; }
        public string? SMSAppName { get; set; }
        //public virtual ICollection<ExmCandidateList>? ExmCandidateLists { get; set; } = new List<ExmCandidateList>();

        #endregion


    }

    public class EcomUser : Base
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? UserRole { get; set; }
        public int? WhId { get; set; }
    }
}
