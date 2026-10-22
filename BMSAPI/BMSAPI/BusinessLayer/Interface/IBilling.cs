
using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    
      public  interface IBilling : ICommonMethod<ProfitShares>
    {
        public int AddIncomeExpense(IncomeExpense incomeExpense);
        public int UpdateIncomeExpenses(IncomeExpense incomeExpense);
    }
}
