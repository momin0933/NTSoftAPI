using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class UserRoleManager : IUserRole
    {
        private readonly ILogger<UserRoleManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_UserRole";

        public UserRoleManager(IDapperService dapperService, ILogger<UserRoleManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool AddUserRole(int uId, string userRole, string entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@UId", uId);
                p.Add("@UserRole", userRole);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("Add user role failed for UId: {UId}, Role: {Role}", uId, userRole);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user role for UId: {UId}, Role: {Role}", uId, userRole);
                throw;
            }
        }
    }
}
