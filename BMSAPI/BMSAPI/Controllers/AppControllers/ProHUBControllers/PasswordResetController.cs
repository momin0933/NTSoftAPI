using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.Models.Apps.PropHUB;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordReset _passwordResetService;
        private readonly ILogger<PasswordResetController> _logger;

        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public PasswordResetController(IPasswordReset passwordResetService, ILogger<PasswordResetController> logger)
        {
            _passwordResetService = passwordResetService ?? throw new ArgumentNullException(nameof(passwordResetService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("api/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Mail))
                    return BadRequest(new { success = false, message = "Email is required" });

                if (!EmailRegex.IsMatch(request.Mail))
                    return BadRequest(new { success = false, message = "Please enter a valid email address" });

                if (!_passwordResetService.CheckEmailExists(request.Mail))
                    return NotFound(new { success = false, message = "No account found with this email address" });

                var sent = await _passwordResetService.SendResetOtpAsync(request.Mail);
                if (!sent)
                    return StatusCode(500, new { success = false, message = "Failed to send reset code, please try again" });

                return Ok(new { success = true, message = "A verification code has been sent to your email" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword for Mail: {Mail}", request?.Mail);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/VerifyResetOtp")]
        public IActionResult VerifyResetOtp([FromBody] VerifyResetOtpRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Mail) || string.IsNullOrWhiteSpace(request.Otp))
                    return BadRequest(new { success = false, message = "Email and code are required" });

                var valid = _passwordResetService.VerifyOtp(request.Mail, request.Otp);
                if (!valid)
                    return BadRequest(new { success = false, message = "Invalid or expired code" });

                return Ok(new { success = true, message = "Code verified" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for Mail: {Mail}", request?.Mail);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("api/ResetPassword")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Mail) || string.IsNullOrWhiteSpace(request.Otp) || string.IsNullOrWhiteSpace(request.NewPassword))
                    return BadRequest(new { success = false, message = "Email, code, and new password are required" });

                if (request.NewPassword.Length < 6)
                    return BadRequest(new { success = false, message = "Password must be at least 6 characters" });

                var success = _passwordResetService.ResetPassword(request.Mail, request.Otp, request.NewPassword);
                if (!success)
                    return BadRequest(new { success = false, message = "Invalid or expired code — please request a new one" });

                return Ok(new { success = true, message = "Password reset successfully — you can now sign in" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for Mail: {Mail}", request?.Mail);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
