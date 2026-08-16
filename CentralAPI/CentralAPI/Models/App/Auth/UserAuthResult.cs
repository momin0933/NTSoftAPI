namespace CentralAPI.Models.App.Auth
{
    public class UserAuthResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public LoggedInUser? User { get; set; }
    }
}
