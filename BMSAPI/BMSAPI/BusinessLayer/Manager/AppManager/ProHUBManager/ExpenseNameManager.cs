using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Apps.PropHUB;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class ExpenseNameManager : IExpenseName
    {
        private readonly ILogger<ExpenseNameManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_ExpenseName";

        public ExpenseNameManager(IDapperService dapperService, ILogger<ExpenseNameManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Universal names + this user's own custom names.
        public IEnumerable<ExpenseNameItem> GetList(int uId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@UId", uId);

                return _IDapperService.GetAllBySP<ExpenseNameItem>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expense names for UId: {UId}", uId);
                throw;
            }
        }

        public ExpenseNameSaveResult Add(AddExpenseNameRequest body)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@UId", body.UId);
                p.Add("@Name", body.Name);
                p.Add("@EntryBy", body.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return ToSaveResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding expense name for UId: {UId}", body.UId);
                throw;
            }
        }

        public ExpenseNameSaveResult Update(UpdateExpenseNameRequest body)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@Id", body.Id);
                p.Add("@UId", body.UId);
                p.Add("@Name", body.Name);
                p.Add("@EntryBy", body.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return ToSaveResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expense name {Id} for UId: {UId}", body.Id, body.UId);
                throw;
            }
        }

        public bool Delete(int id, int uId, string? entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);
                p.Add("@Id", id);
                p.Add("@UId", uId);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                return result != null && Convert.ToInt32(result.AffectedRows) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense name {Id} for UId: {UId}", id, uId);
                throw;
            }
        }

        private static ExpenseNameSaveResult ToSaveResult(dynamic? result)
        {
            if (result == null)
                return new ExpenseNameSaveResult { Id = null, AffectedRows = 0, Status = "Invalid" };

            return new ExpenseNameSaveResult
            {
                Id = result.Id == null ? (int?)null : Convert.ToInt32(result.Id),
                AffectedRows = Convert.ToInt32(result.AffectedRows),
                Status = Convert.ToString(result.Status) ?? string.Empty,
            };
        }
    }
}
