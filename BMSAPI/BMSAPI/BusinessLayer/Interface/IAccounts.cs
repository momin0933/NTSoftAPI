using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IAccounts : ICommonMethod<AccGLedger>
    {
        public Task<AccGLedger> GetById(int id);
    }
}
