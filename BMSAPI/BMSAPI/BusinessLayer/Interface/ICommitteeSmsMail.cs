using BMSAPI.Models;
namespace BMSAPI.BusinessLayer.Interface
{
    public interface ICommitteeSmsMail
    {
        Task<int> CreateCommitteeSmsMail(CommitteeSmSMail committeeSmsMail, List<SmsMailWithMember> members);

        Task<int> CreateCustomerSmsMail(
            CustomerSmsMail customerSmsMail,
            List<SmsMailWithCustomer> members
        );
        Task<CommitteeSmSMail> GetById(int id);
        Task<List<CommitteeSmSMail>> GetAll();
        Task<bool> Update(CommitteeSmSMail committeeSmsMail);
        Task<bool> Delete(int id);
    }
}