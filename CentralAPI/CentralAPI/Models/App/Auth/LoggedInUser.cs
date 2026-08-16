namespace CentralAPI.Models.App.Auth
{
    public class LoggedInUser
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Mail { get; set; }
        public string? UserRole { get; set; }
        public string? ImgPath { get; set; }
        public string? Address { get; set; }
    }
}
