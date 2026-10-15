

using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface ICommitteeMember : ICommonMethod<CommitteeMember>
    {
        int AddCommitteeMembers(int cId, List<CommitteeMember> members);
    }
}
