

using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IBuilding : ICommonMethod<Building>
    {
        public int AddExpenseBill(Expenses entity);
        public List<RptDashboard> GetDashboardData();
        public List<RptExpense> GetAllExpenses(DateTime? fromDate, DateTime? toDate);
        public List<Building> GetAllBuildingFlatOwner();
        public int AddFlat(Flats entity);
        public int UpdateFlat(Flats entity);
        public int RemoveFlat(Flats entity);
        public int DeleteExpense(Expenses expenses);


        public List<RptExpense> GetExpenseById(int id);

    }
}