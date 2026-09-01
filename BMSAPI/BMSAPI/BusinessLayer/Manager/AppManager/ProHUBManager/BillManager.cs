using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Apps.PropHUB;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class BillManager : IBill
    {
        private readonly ILogger<BillManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_Bill";
        public BillManager(IDapperService dapperService, ILogger<BillManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public GenerateBillResult GenerateBill(string phone, string billMonth, string billYear, string entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@Phone", phone);
                p.Add("@BillMonth", billMonth);
                p.Add("@BillYear", billYear);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);

                var output = new GenerateBillResult
                {
                    AffectedRows = (int)result.AffectedRows,
                    AlreadyGenerated = (bool)result.AlreadyGenerated,
                };

                if (output.AlreadyGenerated)
                {
                    _logger.LogWarning("Bill generation skipped — already exists for Phone: {Phone}, {Month} {Year}", phone, billMonth, billYear);
                }
                else
                {
                    _logger.LogInformation("Generated {Count} bill(s) for Phone: {Phone}, {Month} {Year}", output.AffectedRows, phone, billMonth, billYear);
                }

                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating bills for Phone: {Phone}, {Month} {Year}", phone, billMonth, billYear);
                throw;
            }
        }

        public IEnumerable<BillView> GetBillList(string phone, string billMonth, string billYear)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@Phone", phone);
                p.Add("@BillMonth", billMonth);
                p.Add("@BillYear", billYear);

                return _IDapperService.GetAllBySP<BillView>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill list for Phone: {Phone}, {Month} {Year}", phone, billMonth, billYear);
                throw;
            }
        }

        public bool CheckBillExists(string phone, string billMonth, string billYear)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@Phone", phone);
                p.Add("@BillMonth", billMonth);
                p.Add("@BillYear", billYear);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return (bool)result.AlreadyGenerated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking bill existence for Phone: {Phone}, {Month} {Year}", phone, billMonth, billYear);
                throw;
            }
        }

        public bool UpdateBillPayment(int billId, string phone, decimal paidAmount)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);
                p.Add("@BillId", billId);
                p.Add("@Phone", phone);
                p.Add("@PaidAmount", paidAmount);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows > 0)
                {
                    _logger.LogInformation("Bill {BillId} marked as Paid for Phone: {Phone}, amount {Amount}", billId, phone, paidAmount);
                }
                else
                {
                    _logger.LogWarning("Bill payment update affected 0 rows — BillId: {BillId}, Phone: {Phone}", billId, phone);
                }

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bill payment — BillId: {BillId}, Phone: {Phone}", billId, phone);
                throw;
            }
        }
    }
}
