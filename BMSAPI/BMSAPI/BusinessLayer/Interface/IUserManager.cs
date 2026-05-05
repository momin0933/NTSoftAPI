
using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IUserManager
    {
        UserAccount GetUser(string userid, string UserPassword);

    }
}
