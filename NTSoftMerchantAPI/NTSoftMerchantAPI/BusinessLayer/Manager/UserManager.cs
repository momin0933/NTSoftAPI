using Dapper;
using NTSoftMerchantAPI.BusinessLayer.Interface;
using NTSoftMerchantAPI.BusinessLayer.Service;
using NTSoftMerchantAPI.Models;

namespace NTSoftMerchantAPI.BusinessLayer.Manager
{
    public class UserManager : IUser
    {
        private readonly NTSoftDbContext _dbContext;
        private readonly IDapperService _dapperService;  // ✅ Interface use করুন
        private readonly ICommonService _commonService;

        public UserManager(NTSoftDbContext dbContext, IDapperService dapperService)  // ✅ Interface
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dapperService = dapperService ?? throw new ArgumentNullException(nameof(dapperService));
            _commonService = new CommonService(_dbContext);
        }

        public RptUserInfo GetUserInfoByPIN(int PIN)
        {
            string procedur = "SP_UserInfo";
            DynamicParameters p = new DynamicParameters();
            p.Add("@PIN", PIN);
            p.Add("@QueryChecker", 1);
            var UserList = _dapperService.GetAllBySP<RptUserInfo>(procedur, p).ToList();
            return UserList.FirstOrDefault();
        }

        public RptUserInfo GetUserInfoByUserId(string UserId)
        {
            string procedur = "SP_UserInfo";
            DynamicParameters p = new DynamicParameters();
            p.Add("@UserId", UserId);
            p.Add("@QueryChecker", 2);
            var UserList = _dapperService.GetAllBySP<RptUserInfo>(procedur, p).ToList();
            return UserList.FirstOrDefault();
        }

        public UserAccount GetUserAccInfoByUserId(string UserId)
        {
            var user = _commonService.GetAll<UserAccount>().Where(x => x.UserId == UserId).FirstOrDefault();
            return user;
        }

        public List<RptUserInfo> GetAllUserInfo(string UserStatus = "all")
        {
            string procedur = "SP_UserInfo";
            DynamicParameters p = new DynamicParameters();
            p.Add("@QueryChecker", 3);
            
            if (UserStatus == "all")
            {
                p.Add("@IsActive", 2);
            }
            else if (UserStatus == "true")
            {
                p.Add("@IsActive", 1);
            }
            else
            {
                p.Add("@IsActive", 0);
            }

            var UserList = _dapperService.GetAllBySP<RptUserInfo>(procedur, p).ToList();
            return UserList;
        }

        public int AddUser(UserAccount entity)
        {
            try
            {
                return _commonService.Add<UserAccount>(entity);
            }
            catch
            {
                throw;
            }
        }
        
        //public bool RemoveUser(int UserAccId)
        //{
        //    bool ret = false;
        //    try
        //    {
        //        //string RemoveUser = "UPDATE tbluserAccount SET IsActive = false WHERE Id = '" + UserAccId + "'";
        //        //_IDapperService.Post(RemoveUser);
        //        //string RemoveUser1 = "UPDATE tbluserProfile SET IsActive = false WHERE UserAccountId = '" + UserAccId + "'";
        //        //_IDapperService.Post(RemoveUser1);
        //        UserAccount user = _ICommonService.GetAll<UserAccount>().Where(x => x.Id == UserAccId).FirstOrDefault();
        //        user.IsActive = false;
        //        _ICommonService.Update<UserAccount>(user);
        //        UserProfile profile = _ICommonService.GetAll<UserProfile>().Where(x => x.UserAccountId == UserAccId).FirstOrDefault();
        //        profile.IsActive = false;
        //        _ICommonService.Update<UserProfile>(profile);
        //        ret = true;
        //    }
        //    catch(Exception ex)
        //    {
        //        ret = false;
        //    }
        //    return ret;
            

        //}
        //public int AddUserProfile(UserProfile entity)
        //{
        //    try
        //    {
        //        //_dbContext.GLMsts.Add(entity);
        //        //_dbContext.SaveChanges();
        //        return _ICommonService.Add<UserProfile>(entity);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        public int UpdateUser(UserAccount entity)
        {
            try
            {
                //_dbContext.GLMsts.Add(entity);
                //_dbContext.SaveChanges();
                return _commonService.Update<UserAccount>(entity);
            }
            catch
            {
                throw;
            }
        }
        //public int UpdateUserProfile(UserProfile entity)
        //{
        //    try
        //    {
        //        //_dbContext.GLMsts.Add(entity);
        //        //_dbContext.SaveChanges();
        //        return _ICommonService.Update<UserProfile>(entity);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
    }
}
