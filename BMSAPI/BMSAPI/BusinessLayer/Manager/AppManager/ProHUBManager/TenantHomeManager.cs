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
                p.Add("@PropertyNameText", request.PropertyNameText);
                p.Add("@UnitNoText", request.UnitNoText);
                p.Add("@TenantName", request.TenantName);
                p.Add("@NID", request.NID);
                p.Add("@TenantPhone", request.TenantPhone);
                p.Add("@TenantEmail", request.TenantEmail);
                p.Add("@DOB", request.DOB);
                p.Add("@TenantType", request.TenantType);
                p.Add("@Religion", request.Religion);
                p.Add("@StartDate", request.StartDate);
                p.Add("@Advance", request.Advance);
                p.Add("@MonthlyAmount", request.MonthlyAmount);
                p.Add("@PoliceForm", request.PoliceForm);
                p.Add("@AgreementForm", request.AgreementForm);
                p.Add("@EName", request.EName);
                p.Add("@EPhone", request.EPhone);
                p.Add("@ERelation", request.ERelation);
                p.Add("@EAddress", request.EAddress);
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

        public bool UpdateConnection(UpdateTenantHomeConnectionRequest request)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);
                p.Add("@TenantHomeId", request.TenantHomeId);
                p.Add("@UId", request.UId);
                p.Add("@ConnectionStatus", request.ConnectionStatus);
                p.Add("@PropertyId", request.PropertyId);
                p.Add("@PropDetailsId", request.PropDetailsId);
                p.Add("@EntryBy", request.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return Convert.ToInt32(result.AffectedRows) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating connection for TenantHomeId: {Id}", request.TenantHomeId);
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

        public int AddTenantHomeFromTenancy(AddTenantHomeFromTenancyRequest request)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 6);
                p.Add("@UId", request.UId);
                p.Add("@SourceTenantId", request.SourceTenantId);
                p.Add("@EntryBy", request.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;
                return affectedRows <= 0 ? 0 : (int)result.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tenant home from tenancy {Id}", request.SourceTenantId);
                throw;
            }
        }
    }
}
