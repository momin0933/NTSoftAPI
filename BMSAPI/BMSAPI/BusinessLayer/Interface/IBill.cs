

using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IBill:ICommonMethod<Bill>
    {
        public List<RptBill> GetBillByFlatCode(string FlatCode);
        public int UpdateBill(ViewModalBill entity);
        public int AddBill();


    }
}
