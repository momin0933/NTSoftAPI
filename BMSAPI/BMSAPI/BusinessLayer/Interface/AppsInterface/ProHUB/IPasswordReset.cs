namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IPasswordReset
    {
        bool CheckEmailExists(string mail);
        Task<bool> SendResetOtpAsync(string mail);
        bool VerifyOtp(string mail, string otp);
        bool ResetPassword(string mail, string otp, string newPassword);
    }
}
