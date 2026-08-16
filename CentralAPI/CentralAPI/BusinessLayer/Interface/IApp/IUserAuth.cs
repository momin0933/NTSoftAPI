using CentralAPI.Models.App.Auth;

namespace CentralAPI.BusinessLayer.Interface.IApp
{
    public interface IUserAuth
    {
        UserAuthResult? Login(string phone, string password);
        UserAuthResult? RefreshToken(string refreshToken);
        bool Logout(string refreshToken);
    }
}
