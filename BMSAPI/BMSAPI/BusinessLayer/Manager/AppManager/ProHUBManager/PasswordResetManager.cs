using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Apps.PropHUB;

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

        // Reads the logo from wwwroot/images/logo.png and returns it as a
        // base64 data URI, embedded directly into the HTML at send time.
        // This avoids relying on a separate LinkedResource/cid attachment
        // being correctly matched — if the file genuinely isn't found, this
        // returns null and the HTML template simply omits the <img> tag
        // entirely, rather than leaving a broken/dangling image reference.
        private string GetLogoDataUri()
        {
            try
            {
                var webRoot = _webHostEnvironment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    _logger.LogWarning("WebRootPath is not configured — wwwroot may be missing from this project. Sending email without logo.");
                    return null;
                }

                var logoPath = Path.Combine(webRoot, "images", "logo.png");
                _logger.LogInformation("Looking for email logo at: {LogoPath}", logoPath);

                if (!File.Exists(logoPath))
                {
                    _logger.LogWarning("Logo file NOT FOUND at {LogoPath} — check that it was actually deployed to the live server's wwwroot/images folder, not just present locally. Sending email without logo.", logoPath);
                    return null;
                }

                var bytes = File.ReadAllBytes(logoPath);
                _logger.LogInformation("Logo found ({Size} bytes) — embedding as data URI.", bytes.Length);
                return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error reading logo file — sending email without it.");
                return null;
            }
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
                Subject = $"{otp} is your PropHub verification code",
                SubjectEncoding = Encoding.UTF8,
            };
            message.To.Add(toEmail);
            message.ReplyToList.Add(new MailAddress(emailSecret.SenderEmail));

            var year = DateTime.Now.Year;
            var logoDataUri = GetLogoDataUri();

            // Brand colors — matching the app's own theme.js (colors.primaryDark /
            // colors.teal), which the logo itself was designed against.
            const string brandDark = "#006644";
            const string brandTeal = "#0e7c66";

            // The logo has its own white background baked in — placing it inside a
            // white circular badge turns that into a deliberate design element
            // instead of an awkward box sitting on the colored header.
            var logoHtml = logoDataUri != null
                ? $@"<table role=""presentation"" align=""center"" cellpadding=""0"" cellspacing=""0"" style=""margin:0 auto 12px;"">
              <tr>
                <td style=""width:64px;height:64px;border-radius:32px;background-color:#ffffff;text-align:center;vertical-align:middle;"">
                  <img src=""{logoDataUri}"" width=""42"" height=""42"" alt="""" style=""display:block;margin:11px auto;border-radius:8px;"" />
                </td>
              </tr>
            </table>"
                : "";

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
            <td style=""background-color:{brandDark};padding:28px 28px 24px;text-align:center;"">
              {logoHtml}
              <span style=""color:#ffffff;font-size:18px;font-weight:bold;letter-spacing:0.3px;"">PropHub</span>
            </td>
          </tr>
          <tr>
            <td style=""padding:32px 28px;"">
              <p style=""font-size:15px;color:#111827;margin:0 0 16px;"">Hello,</p>
              <p style=""font-size:14px;color:#374151;line-height:22px;margin:0 0 22px;"">
                We received a request to reset the password for your PropHub account. Use the verification code below to continue:
              </p>
              <div style=""background-color:#F0FBF5;border:1px solid {brandTeal};border-radius:10px;padding:20px;text-align:center;margin-bottom:22px;"">
                <span style=""font-size:32px;font-weight:bold;letter-spacing:8px;color:{brandTeal};"">{otp}</span>
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

            message.AlternateViews.Add(plainView);
            message.AlternateViews.Add(htmlView);

            await client.SendMailAsync(message);
        }
        #endregion
    }
}