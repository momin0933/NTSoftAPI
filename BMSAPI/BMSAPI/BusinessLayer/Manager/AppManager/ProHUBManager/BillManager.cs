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
        public RecordBillPaymentResult RecordBillPayment(int billId, string phone, decimal paymentAmount, string paymentType, string remarks, string entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);
                p.Add("@BillId", billId);
                p.Add("@Phone", phone);
                p.Add("@PaidAmount", paymentAmount);
                p.Add("@PaymentType", paymentType);
                p.Add("@Remarks", remarks);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);

                var output = new RecordBillPaymentResult
                {
                    AffectedRows = (int)result.AffectedRows,
                    PaidAmount = result.PaidAmount,
                    Amount = result.Amount,
                    Status = result.Status,
                };

                if (output.AffectedRows > 0)
                {
                    _logger.LogInformation(
                        "Recorded payment of {Amount} ({Type}) for BillId: {BillId}, Phone: {Phone} — new status {Status}",
                        paymentAmount, paymentType, billId, phone, output.Status);
                }
                else
                {
                    _logger.LogWarning("Payment not recorded — BillId: {BillId} not found for Phone: {Phone}", billId, phone);
                }

                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording payment for BillId: {BillId}, Phone: {Phone}", billId, phone);
                throw;
            }
        }

        public IEnumerable<BillPaymentView> GetBillPaymentHistory(string phone, int billId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 5);
                p.Add("@Phone", phone);
                p.Add("@BillId", billId);

                return _IDapperService.GetAllBySP<BillPaymentView>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment history for BillId: {BillId}, Phone: {Phone}", billId, phone);
                throw;
            }
        }
    }
}
