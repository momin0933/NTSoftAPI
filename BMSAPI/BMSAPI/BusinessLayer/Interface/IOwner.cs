

using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IOwner : ICommonMethod<Owner>
    {
        //public IEnumerable<Owner> GetOwners(int Id);
        List<VwServiceCharge> GetFlatWiseOwnerList();
        List<VwServiceCharge> GetFlatWiseServiceCharge(string year, string month);
        VwServiceCharge GetFlatWiseServiceChargeDetails(string month, string year, int FCode);

        public int UpdateOwner(VwServiceCharge entity);
        public int UpdateTenant(VwServiceCharge entity);


    }
}
