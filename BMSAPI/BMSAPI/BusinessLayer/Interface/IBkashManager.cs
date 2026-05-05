using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IBkashManager
    {
        // GET operations (Dapper)
        BkashBillInfo GetBillMonthWise(string UserName, string Password,string FlatCode, string BillMonth);
    }
}
