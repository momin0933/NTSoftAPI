using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Apps.PropHUB;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
  
        public class ComplainManager : IComplain
        {
            private readonly ILogger<ComplainManager> _logger;
            private readonly IDapperService _IDapperService;
            private const string SP_NAME = "SP_Complain";

            public ComplainManager(IDapperService dapperService, ILogger<ComplainManager> logger)
            {
                _IDapperService = dapperService;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public int AddComplain(AddComplainRequest request)
            {
                try
                {
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 1);
                    p.Add("@UId", request.UId);
                    p.Add("@TenantId", request.TenantId);
                    p.Add("@PropertyId", request.PropertyId);
                    p.Add("@PropDetailsId", request.PropDetailsId);
                    p.Add("@Subject", request.Subject);
                    p.Add("@Description", request.Description);
                    p.Add("@EntryBy", request.EntryBy);

                    var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                    int affectedRows = (int)result.AffectedRows;

                    if (affectedRows <= 0)
                    {
                        _logger.LogWarning("Add complain failed for TenantId: {TenantId}", request.TenantId);
                        return 0;
                    }

                    return (int)result.Id;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding complain for TenantId: {TenantId}", request.TenantId);
                    throw;
                }
            }

            public IEnumerable<ComplainView> GetComplainListForAdmin(int uId)
            {
                try
                {
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 2);
                    p.Add("@UId", uId);

                    return _IDapperService.GetAllBySP<ComplainView>(SP_NAME, p).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting complain list for UId: {UId}", uId);
                    throw;
                }
            }

            public IEnumerable<ComplainView> GetComplainListForTenant(int tenantId, int uId)
            {
                try
                {
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 3);
                    p.Add("@TenantId", tenantId);
                    p.Add("@UId", uId);

                    return _IDapperService.GetAllBySP<ComplainView>(SP_NAME, p).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting complain list for TenantId: {TenantId}", tenantId);
                    throw;
                }
            }

            public bool MarkAsRead(int complainId, int uId, string entryBy)
            {
                try
                {
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 4);
                    p.Add("@ComplainId", complainId);
                    p.Add("@UId", uId);
                    p.Add("@EntryBy", entryBy);

                    var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                    return Convert.ToInt32(result.AffectedRows) > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error marking complain {Id} as read", complainId);
                    throw;
                }
            }

            public int GetUnreadCount(int uId)
            {
                try
                {
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 5);
                    p.Add("@UId", uId);

                    var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                    return result == null ? 0 : Convert.ToInt32(result.UnreadCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting unread complain count for UId: {UId}", uId);
                    throw;
                }
            }

            public bool UpdateStatus(int complainId, int uId, string status, string remarks, string entryBy)
            {
                try
                {
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 6);
                    p.Add("@ComplainId", complainId);
                    p.Add("@UId", uId);
                    p.Add("@Status", status);
                    p.Add("@Remarks", remarks);
                    p.Add("@EntryBy", entryBy);

                    var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                    return Convert.ToInt32(result.AffectedRows) > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating complain {Id} status", complainId);
                    throw;
                }
            }
        }
}
