namespace BMSAPI.Models.Apps.PropHUB
{
    public class VerifyResetOtpRequest
    {
        public string Mail { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}
