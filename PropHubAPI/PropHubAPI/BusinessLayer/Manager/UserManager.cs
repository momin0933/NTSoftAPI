using PropHubAPI.BusinessLayer.Interface;
using PropHubAPI.BusinessLayer.Service;
using PropHubAPI.Models;

namespace PropHubAPI.BusinessLayer.Manager
{
    public class UserManager : IUserManager
    {

        private readonly ILogger<UserManager> _logger;
        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;     

        public UserManager(NTSoftDbContextFactory dbContext, IDapperService dapperService, ILogger<UserManager> logger)
        {            
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public UserAccount GetUser(string userid, string UserPassword)
        {
            string Query = "SELECT * FROM tbluserAccount WHERE UserId = @UserId AND password = @Password";
            var parameters = new { UserId = userid, Password = UserPassword };
            return _IDapperService.GetSingle<UserAccount>(Query, parameters);
        }      
    }
}
