using System.Net.Mail;
using System.Net;
using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using Dapper;
using BMSAPI.Models.Apps.PropHUB;
using System.Net.Mime;
using System.Text;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class PasswordResetManager : IPasswordReset
    {
        private readonly ILogger<PasswordResetManager> _logger;
        private readonly IDapperService _IDapperService;
        private readonly IConfiguration _configuration;
        private readonly IUserRegistration _userRegistrationService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private const string SP_RESET = "SP_PasswordReset";
        private const string SP_USER_ACCOUNT = "SP_UserAccount";
        private const string SP_EMAIL_SECRET = "SP_EmailSecret";

        public PasswordResetManager(
            IDapperService dapperService,
            IConfiguration configuration,
            IUserRegistration userRegistrationService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<PasswordResetManager> logger)
        {
            _IDapperService = dapperService;
            _configuration = configuration;
            _userRegistrationService = userRegistrationService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool CheckEmailExists(string mail) => _userRegistrationService.IsEmailExists(mail);

        public async Task<bool> SendResetOtpAsync(string mail)
        {
            try
            {
                var otp = GenerateOtp();

                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@Mail", mail);
                p.Add("@Otp", otp);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_RESET, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("Failed to store reset OTP for Mail: {Mail}", mail);
                    return false;
                }

                await SendOtpEmailAsync(mail, otp);

                _logger.LogInformation("Password reset OTP sent to Mail: {Mail}", mail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reset OTP for Mail: {Mail}", mail);
                throw;
            }
        }

        public bool VerifyOtp(string mail, string otp)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@Mail", mail);
                p.Add("@Otp", otp);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_RESET, p);
                int count = (int)result.RecordCount;

                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for Mail: {Mail}", mail);
                throw;
            }
        }

        public bool ResetPassword(string mail, string otp, string newPassword)
        {
            try
            {
                if (!VerifyOtp(mail, otp))
                {
                    _logger.LogWarning("Reset rejected — invalid/expired OTP for Mail: {Mail}", mail);
                    return false;
                }

                DynamicParameters updateParams = new DynamicParameters();
                updateParams.Add("@QueryChecker", 4);
                updateParams.Add("@Mail", mail);
                updateParams.Add("@Password", newPassword);
                updateParams.Add("@EntryBy", mail);

                var updateResult = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_USER_ACCOUNT, updateParams);
                int affectedRows = (int)updateResult.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("Password update failed for Mail: {Mail}", mail);
                    return false;
                }

                DynamicParameters markUsedParams = new DynamicParameters();
                markUsedParams.Add("@QueryChecker", 3);
                markUsedParams.Add("@Mail", mail);
                markUsedParams.Add("@Otp", otp);
                _IDapperService.GetByDynamicSPSingle<dynamic>(SP_RESET, markUsedParams);

                _logger.LogInformation("Password reset successfully for Mail: {Mail}", mail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for Mail: {Mail}", mail);
                throw;
            }
        }

        #region Helpers

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private EmailSecret GetActiveEmailSecret()
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 1);

            var secret = _IDapperService.GetByDynamicSPSingle<EmailSecret>(SP_EMAIL_SECRET, p);

            if (secret == null || string.IsNullOrWhiteSpace(secret.SenderEmail) || string.IsNullOrWhiteSpace(secret.AppPass))
            {
                _logger.LogError("No active row found in tblEmailSecret — cannot send email.");
                throw new InvalidOperationException("Email sending is not configured. Please contact support.");
            }

            return secret;
        }

        private async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            var emailSecret = GetActiveEmailSecret();

            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderName = _configuration["EmailSettings:SenderName"] ?? "PropHub";

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(emailSecret.SenderEmail, emailSecret.AppPass),
                EnableSsl = true,
            };

            using var message = new MailMessage
            {
                From = new MailAddress(emailSecret.SenderEmail, $"{senderName} (No-Reply)"),
                // Mirrors the pattern real transactional senders (Google,
                // Facebook, etc.) use — the code visible in the subject line
                // itself reads as a familiar, legitimate pattern rather than
                // a generic marketing-style subject.
                Subject = $"{otp} is your PropHub verification code",
                SubjectEncoding = System.Text.Encoding.UTF8,
            };
            message.To.Add(toEmail);
            message.ReplyToList.Add(new MailAddress(emailSecret.SenderEmail));

            var year = DateTime.Now.Year;

            // Plain-text fallback — a proper multipart/alternative structure
            // (both plain text AND html) is a real, recognized deliverability
            // signal that a single-part HTML-only email lacks.
            var plainTextBody = $@"Hello,

We received a request to reset the password for your PropHub account.

Your verification code is: {otp}

This code will expire in 10 minutes.

If you didn't request this, you can safely ignore this email.

This is an automated message from PropHub. Please do not reply.

© {year} PropHub. All rights reserved.";

            var htmlBody = $@"
<html>
<body style=""margin:0;padding:0;background-color:#f4f6f9;font-family:Arial,Helvetica,sans-serif;"">
  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f6f9;padding:28px 0;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,0.06);"">
          <tr>
            <td style=""background-color:#0F9D58;padding:26px 28px;text-align:center;"">
              <img src=""cid:prophub_logo"" width=""46"" height=""46"" alt=""PropHub"" style=""display:block;margin:0 auto 10px;border-radius:10px;"" />
              <span style=""color:#ffffff;font-size:18px;font-weight:bold;letter-spacing:0.3px;"">PropHub</span>
            </td>
          </tr>
          <tr>
            <td style=""padding:32px 28px;"">
              <p style=""font-size:15px;color:#111827;margin:0 0 16px;"">Hello,</p>
              <p style=""font-size:14px;color:#374151;line-height:22px;margin:0 0 22px;"">
                We received a request to reset the password for your PropHub account. Use the verification code below to continue:
              </p>
              <div style=""background-color:#F0FBF5;border:1px solid #0F9D58;border-radius:10px;padding:20px;text-align:center;margin-bottom:22px;"">
                <span style=""font-size:32px;font-weight:bold;letter-spacing:8px;color:#0F9D58;"">{otp}</span>
              </div>
              <p style=""font-size:13px;color:#6B7280;line-height:20px;margin:0 0 4px;"">
                This code will expire in <strong>10 minutes</strong>.
              </p>
              <p style=""font-size:13px;color:#6B7280;line-height:20px;margin:0 0 26px;"">
                If you didn't request a password reset, you can safely ignore this email.
              </p>
              <hr style=""border:none;border-top:1px solid #E5E7EB;margin:0 0 20px;"" />
              <p style=""font-size:11px;color:#9CA3AF;line-height:18px;margin:0;"">
                This is an automated message from PropHub. Please do not reply to this email.
              </p>
            </td>
          </tr>
          <tr>
            <td style=""background-color:#F9FAFB;padding:16px 28px;text-align:center;"">
              <p style=""font-size:11px;color:#9CA3AF;margin:0;"">© {year} PropHub. All rights reserved.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";

            var plainView = AlternateView.CreateAlternateViewFromString(plainTextBody, null, "text/plain");
            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");

            var logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logo.png");
            if (File.Exists(logoPath))
            {
                var logoResource = new LinkedResource(logoPath, "image/png")
                {
                    ContentId = "prophub_logo",
                    TransferEncoding = TransferEncoding.Base64,
                };
                htmlView.LinkedResources.Add(logoResource);
            }
            else
            {
                _logger.LogWarning("Logo file not found at {LogoPath} — email will send without it.", logoPath);
            }

            message.AlternateViews.Add(plainView);
            message.AlternateViews.Add(htmlView);

            await client.SendMailAsync(message);
        }

        #endregion
    }
}
