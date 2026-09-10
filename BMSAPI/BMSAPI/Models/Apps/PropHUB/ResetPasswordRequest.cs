namespace BMSAPI.Models.Apps.PropHUB
{
    public class ResetPasswordRequest
    {
        public string Mail { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
