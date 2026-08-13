using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IUserRegistration
    {
        bool RegisterUser(UserRegistration model);
        bool IsEmailExists(string email);
        bool IsPhoneExists(string phone);
    }
}
