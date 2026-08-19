using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.BusinessLayer.TenantService;
using BMSAPI.Models.Apps.PropHUB;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class TenantManager : ITenant
    {
        private readonly ILogger<TenantManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_Tenant";

        public TenantManager(IDapperService dapperService, ILogger<TenantManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

     
        public IEnumerable<TenantFullView> GetTenantList(string phone)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@Phone", phone);

                return _IDapperService.GetAllBySP<TenantFullView>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tenant list for Phone: {Phone}", phone);
                throw;
            }
        }

        public bool AddTenant(TenantData model)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@Phone", model.Phone);
                p.Add("@PropertyId", model.PropertyId);
                p.Add("@PropDetailsId", model.PropDetailsId);
                p.Add("@TenantName", model.TenantName);
                p.Add("@NID", model.NID);
                p.Add("@TenantPhone", model.TenantPhone);
                p.Add("@TenantEmail", model.TenantEmail);
                p.Add("@DOB", model.DOB);
                p.Add("@TenantType", model.TenantType);
                p.Add("@Religion", model.Religion);
                p.Add("@StartDate", model.StartDate);
                p.Add("@EndDate", model.EndDate);
                p.Add("@Advance", model.Advance);
                p.Add("@MonthlyAmount", model.MonthlyAmount);
                p.Add("@PoliceForm", model.PoliceForm);
                p.Add("@AgreementForm", model.AgreementForm);
                p.Add("@EName", model.EName);
                p.Add("@EPhone", model.EPhone);
                p.Add("@ERelation", model.ERelation);
                p.Add("@EAddress", model.EAddress);
                p.Add("@Remarks", model.Remarks);
                p.Add("@EntryBy", model.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("Add tenant failed for TenantPhone: {TenantPhone}", model.TenantPhone);
                    return false;
                }

                _logger.LogInformation("Tenant added successfully with Id: {Id}, TenantPhone: {TenantPhone}", (int)result.Id, model.TenantPhone);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tenant with TenantPhone: {TenantPhone}", model.TenantPhone);
                throw;
            }
        }
    }
}
