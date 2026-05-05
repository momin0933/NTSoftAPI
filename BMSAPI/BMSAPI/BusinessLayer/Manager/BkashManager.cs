using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Http; // Add this at the top if not already present

namespace BMSAPI.BusinessLayer.Manager
{
    public class BkashManager: IBkashManager
    {
        private readonly ILogger<BkashManager> _logger;
        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        IUserManager _userData;
        private readonly IHttpContextAccessor _httpContextAccessor; // Add this field

        public BkashManager(
            NTSoftDbContextFactory dbContext,
            IDapperService dapperService,
            ILogger<BkashManager> logger,
            IUserManager userData,
            IHttpContextAccessor httpContextAccessor // Add this parameter
        )
        {            
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userData = userData ?? throw new ArgumentNullException(nameof(userData));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor)); // Assign
        }
        
        #region GET Operations (Dapper)
        public BkashBillInfo GetBillMonthWise(string UserName, string Password, string FlatCode, string BillMonth)
        {
            try
            {
                if (UserName != null && Password != null)
                {
                    // 1. Validate user
                    var user = _userData.GetUser(UserName, Password);                    

                    if (user == null)
                    {
                        _logger.LogWarning("User authentication failed for: {UserName}", UserName);
                        return null;
                    }

                    // Use IHttpContextAccessor to access the current HttpContext
                    _httpContextAccessor.HttpContext?.Session.SetString("TenantId", user.TenantId);
                    string procedur = "SP_GetBillForBkash";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@FlatCode", FlatCode);
                    p.Add("@BillMonth", BillMonth);
                    BkashBillInfo billInfo = _IDapperService.GetAllBySP<BkashBillInfo>(procedur, p).FirstOrDefault();
                    _httpContextAccessor.HttpContext?.Session.Remove("TenantId");                    
                    return billInfo;
                }

                _logger.LogWarning("Invalid input: UserName or Password is null");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill information");
                throw;
            }
        }
        
        #endregion
    }
}
