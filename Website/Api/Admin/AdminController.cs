using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Data;
using App.Extensions;
using MakC.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Website.Api.Admin
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [AdminAuthorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        public string test()
        {
            return "asdasdasdasd";
        }
        /// <summary>
        /// 登录事件
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ApiResult<string> Login(string loginname, string password)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                var managerobj = dbh.GetEntityDB<Manager>().AsQueryable().First(ii => ii.username == loginname);
                if (mUtils.MD5Hash(password??"") != managerobj.password)
                {
                    apiRes.msg = "账号或密码错误";
                    return apiRes;
                }
                var identity = new ClaimsPrincipal(
                       new ClaimsIdentity(new[]
                           {
                            new Claim(ClaimTypes.Sid, managerobj.id.ToString()),
                            new Claim(ClaimTypes.Role,"Admin"),
                            new Claim(ClaimTypes.Name, managerobj.username),
                            new Claim(ClaimTypes.WindowsAccountName,managerobj.username)
                           }, AdminAuthorizeAttribute.AuthenticationScheme)
                   );
                HttpContext.SignInAsync(AdminAuthorizeAttribute.AuthenticationScheme, identity,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(7)) // 有效时间
                    });
                apiRes.ok = true;                
            }
            catch (Exception ex)
            {
                apiRes.ok = false;
                apiRes.msg = ex.Message;
            }            
            return apiRes;
        }

        /// <summary>
        /// 管理员退出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ApiResult<string> LogOut()
        {
            HttpContext.SignOutAsync(AdminAuthorizeAttribute.AuthenticationScheme);
            return new ApiResult<string>() { data = "/admin/login/" };
        }
    }
}