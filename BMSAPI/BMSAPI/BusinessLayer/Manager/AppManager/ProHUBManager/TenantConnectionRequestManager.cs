using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Apps.PropHUB;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class TenantConnectionRequestManager : ITenantConnectionRequest
    {
        private readonly ILogger<TenantConnectionRequestManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_TenantConnectionRequest";

        // The landlord-invite methods below (SendConnectInvite through
        // GetConnectedTenanciesForTenant) belong to a completely
        // different mechanism — SP_Tenant's QC10-13, not
        // SP_TenantConnectionRequest — hosted here because this is the
        // one class/controller pair already confirmed live and wired
        // into DI.
        private const string SP_TENANT = "SP_Tenant";

        public TenantConnectionRequestManager(IDapperService dapperService, ILogger<TenantConnectionRequestManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int SendRequest(SendConnectionRequestBody body)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@TenantUId", body.TenantUId);
                p.Add("@TenantHomeId", body.TenantHomeId);
                p.Add("@LandlordMobile", body.LandlordMobile);
                p.Add("@TenancyId", body.TenancyId);
                p.Add("@PropertyId", body.PropertyId);
                p.Add("@PropDetailsId", body.PropDetailsId);
                p.Add("@MatchType", body.MatchType);
                p.Add("@EntryBy", body.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;
                int newId = affectedRows <= 0 ? 0 : (int)result.Id;

                if (newId > 0 && body.MatchType == "Auto")
                {
                    Accept(newId, body.LandlordUId ?? 0, body.EntryBy ?? "system");
                }

                return newId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending connection request for TenantUId: {UId}", body.TenantUId);
                throw;
            }
        }

        public IEnumerable<ConnectionRequestItem> GetForLandlord(int landlordUId, string landlordMobile)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@LandlordUId", landlordUId);
                p.Add("@LandlordMobile", landlordMobile);

                return _IDapperService.GetAllBySP<ConnectionRequestItem>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting requests for LandlordUId: {UId}", landlordUId);
                throw;
            }
        }

        public IEnumerable<ConnectionRequestItem> GetForTenant(int tenantUId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@TenantUId", tenantUId);

                return _IDapperService.GetAllBySP<ConnectionRequestItem>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting requests for TenantUId: {UId}", tenantUId);
                throw;
            }
        }

        public bool Accept(int requestId, int landlordUId, string entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);
                p.Add("@RequestId", requestId);
                p.Add("@LandlordUId", landlordUId);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return Convert.ToInt32(result.AffectedRows) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting request {Id}", requestId);
                throw;
            }
        }

        public bool Reject(int requestId, string entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 5);
                p.Add("@RequestId", requestId);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return Convert.ToInt32(result.AffectedRows) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting request {Id}", requestId);
                throw;
            }
        }

        public AutoMatchResult? CheckAutoMatch(string tenantPhone)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 6);
                p.Add("@TenantPhone", tenantPhone);

                return _IDapperService.GetByDynamicSPSingle<AutoMatchResult>(SP_NAME, p);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking auto-match for phone: {Phone}", tenantPhone);
                throw;
            }
        }

        public bool SendConnectInvite(int uId, int tenantId, string entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 10);
                p.Add("@UId", uId);
                p.Add("@TenantId", tenantId);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_TENANT, p);
                if (result == null) return false;

                int? matchedUId = result.MatchedUId;
                int affectedRows = Convert.ToInt32(result.AffectedRows);

                return matchedUId.HasValue && affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending connect invite for TenantId: {TenantId}", tenantId);
                throw;
            }
        }

        public IEnumerable<TenantFullView> GetPendingInvitesForTenant(int tenantUserId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 11);
                p.Add("@UId", tenantUserId);

                return _IDapperService.GetAllBySP<TenantFullView>(SP_TENANT, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending invites for TenantUserId: {UId}", tenantUserId);
                throw;
            }
        }

        public bool RespondToInvite(int tenantId, int tenantUserId, bool accept, string entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 12);
                p.Add("@TenantId", tenantId);
                p.Add("@UId", tenantUserId);
                p.Add("@Accept", accept);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_TENANT, p);
                return Convert.ToInt32(result.AffectedRows) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error responding to invite for TenantId: {TenantId}", tenantId);
                throw;
            }
        }

        public IEnumerable<TenantFullView> GetConnectedTenanciesForTenant(int tenantUserId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 13);
                p.Add("@UId", tenantUserId);

                return _IDapperService.GetAllBySP<TenantFullView>(SP_TENANT, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting connected tenancies for TenantUserId: {UId}", tenantUserId);
                throw;
            }
        }
    }
}