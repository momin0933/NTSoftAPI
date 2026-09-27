using PropHubAPI.Models.Apps.PropHUB;

namespace PropHubAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IUserRegistration
    {
        int RegisterUser(UserRegistration model);
        bool IsEmailExists(string email);
        bool IsPhoneExists(string phone);
    }
}
