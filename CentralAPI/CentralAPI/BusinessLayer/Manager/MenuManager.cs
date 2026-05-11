using CentralAPI.BusinessLayer.Interface;
using CentralAPI.BusinessLayer.Service;
using CentralAPI.Models.ReportModels;
using Dapper;

namespace CentralAPI.BusinessLayer.Manager
{
    public class MenuManager : IMenu
    {
        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        private readonly ICommonService _ICommonService;
        private readonly ILogger<MenuManager> _logger;
        public MenuManager(
            NTSoftDbContextFactory dbContext,
            IDapperService dapperService,
            ILogger<MenuManager> logger)
        {
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<RptMenu> GetAllMenuListWithUserRole(string UserRole, string projectName)
        {
            try
            {

                string SP = "SP_GETMENU";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@UserRole", UserRole);
                p.Add("@projectName", projectName);
                List<RptMenu> ItemList = _IDapperService.GetAllBySP<RptMenu>(SP, p).OrderBy(x => x.ModuleSorting).ToList();
                return ItemList;


            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
