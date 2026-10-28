using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IAcctblVoucher : ICommonMethod<AccVoucher>
    {
        public Task<AccVoucher> GetById(int Id);
        public RptAccVoucher GetVocherById(int id);
        public List<RptAccVoucherListWithDetails> ACCGetAllVoucherListWithDetails(DateTime fromDate, DateTime toDate, string? vhoucherType, string? voucherNumber, int companyId);
        public List<string> GetVoucherNoByVoucherType(DateTime fromDate, DateTime toDate, string voucherType);
        public int UpdateOrAdd(AccVoucher entity);

    }
}
