

using NTSoftMerchantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTSoftMerchantAPI.BusinessLayer.Interface
{
    public interface IUser
    {
        RptUserInfo GetUserInfoByPIN(int PIN);
        RptUserInfo GetUserInfoByUserId(string UserId);
        UserAccount GetUserAccInfoByUserId(string UserId);
        List<RptUserInfo> GetAllUserInfo(string UserStatus = "all");
        int AddUser(UserAccount entity);
        //int AddUserProfile(UserProfile entity);
        int UpdateUser(UserAccount entity);
        //int UpdateUserProfile(UserProfile entity);
       // bool RemoveUser(int UserAccId);
    }
}
