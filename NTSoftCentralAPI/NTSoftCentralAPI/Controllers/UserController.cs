using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NTSoftCentralAPI.BusinessLayer.Interface;
using NTSoftCentralAPI.Models;
using System.Security.Claims;

namespace NTSoftCentralAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _IUser;

        public UserController(IUser IUser)
        {
            _IUser = IUser;
        }
        [Route("api/GetUserInfoByPIN")]
        [HttpGet]
        public RptUserInfo GetUserInfoByPIN([FromQuery] int PIN)
        {
            try
            {
                RptUserInfo userinfo = _IUser.GetUserInfoByPIN(PIN);
                return userinfo;
            }
            catch
            {
                throw;
            }
        }
        [Route("api/GetUserInfoByUserId")]
        [HttpGet]
        public RptUserInfo GetUserInfoByUserId([FromQuery] string UserId)
        {
            try
            {
                //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //var tenantId = User.FindFirst("TenantId")?.Value;
                //var role = User.FindFirst(ClaimTypes.Role)?.Value;
                RptUserInfo userinfo = _IUser.GetUserInfoByUserId(UserId);
                return userinfo;
            }
            catch
            {
                throw;
            }
        }
        [Route("api/GetUserInfoByEmail")]
        [HttpGet]
        public RptUserInfo GetUserInfoByEmail([FromQuery] string UserId)
        {
            try
            {
                RptUserInfo userinfo = _IUser.GetAllUserInfo().Where(x=> x.UserId == UserId).FirstOrDefault();
                return userinfo;
            }
            catch
            {
                throw;
            }
        }
        [Route("api/GetAllUserInfo")]
        [HttpGet]
        public List<RptUserInfo> GetAllUserInfo([FromQuery] string Stauts="all")
        {
            try
            {                

                List<RptUserInfo> userlist = new List<RptUserInfo>();
                userlist = _IUser.GetAllUserInfo(Stauts).ToList();
                return userlist;
            }
            catch
            {
                throw;
            }
        }

        [Route("api/GetAllUserInfoByUserRole")]
        [HttpGet]
        public List<RptUserInfo> GetAllUserInfoByUserRole([FromQuery] string userrole)
        {
            try
            {

                List<RptUserInfo> userlist = new List<RptUserInfo>();
                userlist = _IUser.GetAllUserInfo("true").ToList();
                userlist = userlist.FindAll(x => x.userrole == userrole).ToList();
                return userlist;
            }
            catch
            {
                throw;
            }
        }

      
    }
}
