using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CentralAPI.BusinessLayer.Interface.IApp;
using CentralAPI.BusinessLayer.Service;
using CentralAPI.Models.App.Auth;
using Dapper;
using Microsoft.IdentityModel.Tokens;

namespace CentralAPI.BusinessLayer.Manager.AppManager
{
    public class UserAuthManager : IUserAuth
    {
        private readonly ILogger<UserAuthManager> _logger;
        private readonly IDapperService _IDapperService;
        private readonly IConfiguration _configuration;

        private const string SP_USER_LOGIN = "SP_UserLogin";
        private const string SP_TOKEN = "SP_UserRefreshToken";

        private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        public UserAuthManager(IDapperService dapperService, IConfiguration configuration, ILogger<UserAuthManager> logger)
        {
            _IDapperService = dapperService;
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public UserAuthResult? Login(string phone, string password)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@Phone", phone);
                p.Add("@Password", password);

                var user = _IDapperService.GetByDynamicSPSingle<LoggedInUser>(SP_USER_LOGIN, p);
                if (user == null)
                {
                    _logger.LogWarning("Login failed — no matching user for Phone: {Phone}", phone);
                    return null;
                }

                return IssueTokens(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for Phone: {Phone}", phone);
                throw;
            }
        }

        public UserAuthResult? RefreshToken(string refreshToken)
        {
            try
            {
                DynamicParameters lookup = new DynamicParameters();
                lookup.Add("@QueryChecker", 2);
                lookup.Add("@Token", refreshToken);

                var stored = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_TOKEN, lookup);
                if (stored == null)
                {
                    _logger.LogWarning("Refresh failed — token not found");
                    return null;
                }

                bool isRevoked = (bool)stored.IsRevoked;
                DateTime expiryDate = (DateTime)stored.ExpiryDate;
                string userIdString = (string)stored.UserId;

                if (isRevoked)
                {
                    _logger.LogWarning("Refresh failed — token revoked. UserId: {UserId}", userIdString);
                    return null;
                }
                if (expiryDate < DateTime.UtcNow)
                {
                    _logger.LogWarning("Refresh failed — token expired. UserId: {UserId}", userIdString);
                    return null;
                }
                if (!int.TryParse(userIdString, out int userId))
                {
                    _logger.LogWarning("Refresh failed — stored UserId is not a valid int: {UserId}", userIdString);
                    return null;
                }

                // Revoke the used token — refresh tokens are single-use
                // (rotated on every refresh) to limit damage if one leaks.
                DynamicParameters revoke = new DynamicParameters();
                revoke.Add("@QueryChecker", 3);
                revoke.Add("@Token", refreshToken);
                _IDapperService.GetByDynamicSPSingle<dynamic>(SP_TOKEN, revoke);

                DynamicParameters getUser = new DynamicParameters();
                getUser.Add("@QueryChecker", 2);
                getUser.Add("@Id", userId);

                var user = _IDapperService.GetByDynamicSPSingle<LoggedInUser>(SP_USER_LOGIN, getUser);
                if (user == null)
                {
                    _logger.LogWarning("Refresh failed — user no longer exists. UserId: {UserId}", userId);
                    return null;
                }

                return IssueTokens(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                throw;
            }
        }

        public bool Logout(string refreshToken)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@Token", refreshToken);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_TOKEN, p);
                int affectedRows = (int)result.AffectedRows;
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                throw;
            }
        }

        #region Token issuing helpers

        private UserAuthResult IssueTokens(LoggedInUser user)
        {
            var expiration = DateTime.UtcNow.Add(AccessTokenLifetime);
            var accessToken = GenerateAccessToken(user, expiration);
            var refreshToken = GenerateRefreshTokenString();

            DynamicParameters insert = new DynamicParameters();
            insert.Add("@QueryChecker", 1);
            insert.Add("@UserId", user.Id.ToString());
            insert.Add("@Token", refreshToken);
            insert.Add("@ExpiryDate", DateTime.UtcNow.Add(RefreshTokenLifetime));
            insert.Add("@EntryBy", user.Phone);
            _IDapperService.GetByDynamicSPSingle<dynamic>(SP_TOKEN, insert);

            return new UserAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = expiration,
                User = user,
            };
        }

        private string GenerateAccessToken(LoggedInUser user, DateTime expiration)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Role, user.UserRole ?? string.Empty),
                new Claim("Phone", user.Phone ?? string.Empty),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshTokenString()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        #endregion
    }
}
