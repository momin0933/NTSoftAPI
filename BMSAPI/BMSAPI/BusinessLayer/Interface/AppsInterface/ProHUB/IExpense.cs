using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IExpense
    {
        int AddExpense(AddExpenseRequest request);
        IEnumerable<ExpenseView> GetExpenseList(string phone, int? expenseMonth, int? expenseYear);
        IEnumerable<ExpenseSourceOption> GetExpenseSourceOptions(string phone);
        bool DeleteExpense(string phone, int expenseId, string updateBy);
    }
}
