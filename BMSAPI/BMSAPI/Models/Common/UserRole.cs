namespace BMSAPI.Models.Common
{
    public class UserRoles : Base
    {       
        public string UserRole { get; set; }       
        public string? ProjectName { get; set; }
        public int CompId { get; set; }
    }
}
