using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Apps.PropHUB;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class TenantHomeManager : ITenantHome
    {
        private readonly ILogger<TenantHomeManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_TenantHome";

        public TenantHomeManager(IDapperService dapperService, ILogger<TenantHomeManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int AddTenantHome(AddTenantHomeRequest request)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@UId", request.UId);
                p.Add("@PropertyName", request.PropertyName);
                p.Add("@Address", request.Address);
                p.Add("@Area", request.Area);
                p.Add("@UnitNo", request.UnitNo);
                p.Add("@MonthlyRent", request.MonthlyRent);
                p.Add("@RentDueDay", request.RentDueDay);
                p.Add("@MoveInDate", request.MoveInDate);
                p.Add("@EntryBy", request.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;
                return affectedRows <= 0 ? 0 : (int)result.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tenant home for UId: {UId}", request.UId);
                throw;
            }
        }

        public TenantHomeItem? GetMyCurrentHome(int uId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@UId", uId);

                return _IDapperService.GetByDynamicSPSingle<TenantHomeItem>(SP_NAME, p);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current home for UId: {UId}", uId);
                throw;
            }
        }

        public IEnumerable<TenantHomeItem> GetMyHomeHistory(int uId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@UId", uId);

                return _IDapperService.GetAllBySP<TenantHomeItem>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting home history for UId: {UId}", uId);
                throw;
            }
        }

        public bool UpdateConnection(int tenantHomeId, int uId, string connectionStatus, int? linkedTenancyId, string entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);
                p.Add("@TenantHomeId", tenantHomeId);
                p.Add("@UId", uId);
                p.Add("@ConnectionStatus", connectionStatus);
                p.Add("@LinkedTenancyId", linkedTenancyId);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return Convert.ToInt32(result.AffectedRows) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating connection for TenantHomeId: {Id}", tenantHomeId);
                throw;
            }
        }

        public bool DeactivateMyHome(int tenantHomeId, int uId, string entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 5);
                p.Add("@TenantHomeId", tenantHomeId);
                p.Add("@UId", uId);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return Convert.ToInt32(result.AffectedRows) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating home {Id}", tenantHomeId);
                throw;
            }
        }
    }
}
