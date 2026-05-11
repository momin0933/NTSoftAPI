using CentralAPI.BusinessLayer.Interface;
using CentralAPI.BusinessLayer.Service;
using CentralAPI.Models;
using Dapper;

namespace CentralAPI.BusinessLayer.Manager
{
    public class DropdownManager : IDropdown
    {
        private const string SP = "SP_GET_DROPDOWN";

        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        private readonly ICommonService _ICommonService;
        private readonly ILogger<DropdownManager> _logger;

        public DropdownManager(
            NTSoftDbContextFactory dbContext,
            IDapperService dapperService,
            ILogger<DropdownManager> logger)
        {
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<BuyerDropdown> GetAllBuyers()
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                return _IDapperService.GetAllBySP<BuyerDropdown>(SP, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching buyers");
                return null;
            }
        }

        public List<CategoryDropdown> GetAllCategories()
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                return _IDapperService.GetAllBySP<CategoryDropdown>(SP, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                return null;
            }
        }

        public List<CustomerDropdown> GetAllCustomers()
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                return _IDapperService.GetAllBySP<CustomerDropdown>(SP, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customers");
                return null;
            }
        }

        public List<DepartmentDropdown> GetAllDepartments()
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);
                return _IDapperService.GetAllBySP<DepartmentDropdown>(SP, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching departments");
                return null;
            }
        }

        public List<FactoryDropdown> GetAllFactories()
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 5);
                return _IDapperService.GetAllBySP<FactoryDropdown>(SP, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching factories");
                return null;
            }
        }
    }
}
