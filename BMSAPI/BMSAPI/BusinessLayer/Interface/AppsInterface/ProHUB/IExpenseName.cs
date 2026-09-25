using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IExpenseName
    {
        IEnumerable<ExpenseNameItem> GetList(int uId);
        ExpenseNameSaveResult Add(AddExpenseNameRequest body);
        ExpenseNameSaveResult Update(UpdateExpenseNameRequest body);
        bool Delete(int id, int uId, string? entryBy);
    }
}
