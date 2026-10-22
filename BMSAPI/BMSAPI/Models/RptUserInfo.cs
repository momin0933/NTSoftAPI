namespace BMSAPI.Models
{
    public class RptUserAccount
    {
        public string UserId { get; set; }
        public string password { get; set; }
    }
    public class RptUserInfo : RptUserAccount
    {
        public int Id { get; set; }
        public int? MemberId { get; set; }
        public int? ProfileId { get; set; }
        public string? Member { get; set; }
        //public string? userid { get; set; }      
        public string? userrole { get; set; }
        public int? PIN { get; set; }
        public string? fullname { get; set; }
        public string? username { get; set; }
        public string? Class { get; set; }
        public string? grade { get; set; }
        public string? filepath { get; set; }
        public string? filename { get; set; }
        public string? userstatus { get; set; }
        public bool? IsActive { get; set; }
        public string? SMSAppName { get; set; }

    }

    public class EcomRptUserAccount
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class SocialLoginRequest
    {
        public string Provider { get; set; } // "Google" | "Facebook"
        public string Credential { get; set; } // Google ID token, or Facebook access token
    }

}
