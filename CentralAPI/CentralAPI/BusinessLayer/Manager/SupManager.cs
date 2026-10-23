using CentralAPI.BusinessLayer.Interface;
using CentralAPI.BusinessLayer.Service;
using CentralAPI.Models.ReportModels;
using Dapper;

namespace CentralAPI.BusinessLayer.Manager
{
    public class SupManager : ISupBill
    {
        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        private readonly ICommonService _ICommonService;
        private readonly ILogger<MenuManager> _logger;
        public SupManager(
            NTSoftDbContextFactory dbContext,
            IDapperService dapperService,
            ILogger<MenuManager> logger)
        {
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public List<RptSubBill> GetSubBill(string projectName, DateTime invoiceDt)
        {
            try
            {
                string SP = "SP_GetSubBill";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@ProjectName", projectName);
                p.Add("@InvoiceDt", invoiceDt.Date);
                List<RptSubBill> itemList = _IDapperService.GetAllBySP<RptSubBill>(SP, p).ToList();
                return itemList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription bills");
                throw;
            }
        }
    }
}
