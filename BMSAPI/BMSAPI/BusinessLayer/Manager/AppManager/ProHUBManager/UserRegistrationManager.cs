using System;
using Dapper;
using Microsoft.Extensions.Logging;
using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class UserRegistrationManager : IUserRegistration
    {
        private readonly ILogger<UserRegistrationManager> _logger;
        private readonly IDapperService _IDapperService;

        private const string SP_NAME = "SP_UserAccount";

        public UserRegistrationManager(IDapperService dapperService, ILogger<UserRegistrationManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region INSERT Operation (Dapper) — QueryChecker = 1

        public bool RegisterUser(UserRegistration model)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@Name", model.Name);
                p.Add("@Phone", model.Phone);
                p.Add("@Mail", model.Mail);
                p.Add("@Password", model.Password);
                p.Add("@UserRole", model.UserRole);
                p.Add("@ImgPath", model.ImgPath);
                p.Add("@Address", model.Address);
                p.Add("@Remarks", model.Remarks);
                p.Add("@EntryBy", model.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("User registration failed for Mail: {Mail}", model.Mail);
                    return false;
                }

                _logger.LogInformation("User registered successfully with Id: {Id}, Mail: {Mail}", (int)result.Id, model.Mail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user with Mail: {Mail}", model.Mail);
                throw;
            }
        }

        #endregion

        #region Duplicate Checks (Dapper) — QueryChecker = 2, 3

        public bool IsEmailExists(string email)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@Mail", email);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int count = (int)result.RecordCount;

                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email existence: {Mail}", email);
                throw;
            }
        }

        public bool IsPhoneExists(string phone)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@Phone", phone);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int count = (int)result.RecordCount;

                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking phone existence: {Phone}", phone);
                throw;
            }
        }

        #endregion
    }
}