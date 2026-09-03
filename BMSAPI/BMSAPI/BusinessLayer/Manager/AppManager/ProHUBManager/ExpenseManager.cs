using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Apps.PropHUB;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class ExpenseManager : IExpense
    {
        private readonly ILogger<ExpenseManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_Expense";
        public ExpenseManager(IDapperService dapperService, ILogger<ExpenseManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public int AddExpense(AddExpenseRequest request)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@Phone", request.Phone);
                p.Add("@ExpenseType", request.ExpenseType);
                p.Add("@ExpenseName", request.ExpenseName);
                p.Add("@PropertyId", request.PropertyId);
                p.Add("@PropDetailsId", request.PropDetailsId);
                p.Add("@TenantId", request.TenantId);
                p.Add("@ExpenseAmount", request.ExpenseAmount);
                p.Add("@ExpenseDate", request.ExpenseDate);
                p.Add("@PaymentType", request.PaymentType);
                p.Add("@Remarks", request.Remarks);
                p.Add("@EntryBy", request.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int newId = Convert.ToInt32(result.NewId);

                _logger.LogInformation("Expense {Id} added for Phone: {Phone}, amount {Amount}",
                    newId, request.Phone, request.ExpenseAmount);

                return newId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding expense for Phone: {Phone}", request?.Phone);
                throw;
            }
        }

        public IEnumerable<ExpenseView> GetExpenseList(string phone, int? expenseMonth, int? expenseYear)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@Phone", phone);
                p.Add("@ExpenseMonth", expenseMonth);
                p.Add("@ExpenseYear", expenseYear);

                return _IDapperService.GetAllBySP<ExpenseView>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expense list for Phone: {Phone}", phone);
                throw;
            }
        }

        public IEnumerable<ExpenseSourceOption> GetExpenseSourceOptions(string phone)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@Phone", phone);

                return _IDapperService.GetAllBySP<ExpenseSourceOption>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expense source options for Phone: {Phone}", phone);
                throw;
            }
        }

        public bool DeleteExpense(string phone, int expenseId, string updateBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);
                p.Add("@Phone", phone);
                p.Add("@ExpenseId", expenseId);
                p.Add("@EntryBy", updateBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return Convert.ToInt32(result.AffectedRows) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense {Id} for Phone: {Phone}", expenseId, phone);
                throw;
            }
        }
    }
}
