namespace BMSAPI.Models.Apps.PropHUB
{
    public class UserRegistration:Base
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Mail { get; set; }
        public string? Password { get; set; }
        public string? UserRole { get; set; }
        public string? ImgPath { get; set; }
        public string? Address { get; set; }
    }
}
