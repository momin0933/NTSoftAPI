
using PropHubAPI.Models;

namespace PropHubAPI.BusinessLayer.Interface
{
    public interface IUserManager
    {
        UserAccount GetUser(string userid, string UserPassword);

    }
}
