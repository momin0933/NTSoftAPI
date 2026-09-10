using System.Net.Mail;
using System.Net;
using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using Dapper;
using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class PasswordResetManager : IPasswordReset
    {
        private readonly ILogger<PasswordResetManager> _logger;
        private readonly IDapperService _IDapperService;
        private readonly IConfiguration _configuration;
        private readonly IUserRegistration _userRegistrationService;

        private const string SP_RESET = "SP_PasswordReset";
        private const string SP_USER_ACCOUNT = "SP_UserAccount";
        private const string SP_EMAIL_SECRET = "SP_EmailSecret";

        public PasswordResetManager(
            IDapperService dapperService,
            IConfiguration configuration,
            IUserRegistration userRegistrationService,
            ILogger<PasswordResetManager> logger)
        {
            _IDapperService = dapperService;
            _configuration = configuration;
            _userRegistrationService = userRegistrationService;
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

        // Fetches the active sender email + app password from tblEmailSecret,
        // instead of appsettings.json — lets the sending account be rotated
        // by updating a database row, with no redeploy needed.
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

            // SmtpHost/Port/SenderName are plain config, not secrets — these
            // still come from appsettings.json.
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderName = _configuration["EmailSettings:SenderName"] ?? "PropHub";

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(emailSecret.SenderEmail, emailSecret.AppPass),
                EnableSsl = true,
            };

            var message = new MailMessage
            {
                // The display name signals "no-reply" — this is the standard
                // convention, but it's not a technical block: Gmail SMTP
                // requires the From address to be a real, deliverable
                // mailbox, so a reply from the recipient's email client will
                // still land in that Gmail inbox regardless of the label.
                From = new MailAddress(emailSecret.SenderEmail, $"{senderName} (No-Reply)"),
                Subject = "Your PropHub password reset code — please do not reply",
                Body = $@"
                    <div style='font-family:Arial,sans-serif;padding:20px;'>
                        <h2 style='color:#0F9D58;'>PropHub password reset</h2>
                        <p>Your verification code is:</p>
                        <h1 style='letter-spacing:4px;color:#111827;'>{otp}</h1>
                        <p>This code expires in 10 minutes.</p>
                        <hr style='border:none;border-top:1px solid #E5E7EB;margin:20px 0;' />
                        <p style='color:#9CA3AF;font-size:12px;'>
                            This is an automated message from PropHub. This mailbox is not
                            monitored — please do not reply. If you didn't request this code,
                            you can safely ignore this email.
                        </p>
                    </div>",
                IsBodyHtml = true,
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }

        #endregion
    }
}
