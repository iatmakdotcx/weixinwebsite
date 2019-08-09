using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
            var token = "";
            try
            {

                var identity = new ClaimsPrincipal(
                    new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Sid,"0123456789"),
                            new Claim(ClaimTypes.Role,"Admin"),
                            new Claim(ClaimTypes.Name,"admin's name"),
                            new Claim(ClaimTypes.WindowsAccountName,"admin"),
                            new Claim(ClaimTypes.UserData,"user.UpLoginDate.ToString()")
                        }, AdminAuthorizeAttribute.AuthenticationScheme)
                );
                HttpContext.SignInAsync(AdminAuthorizeAttribute.AuthenticationScheme, identity,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(7)) // 有效时间
                    });
            }
            catch (Exception ex)
            {
                
            }
            apiRes.ok = true;
            apiRes.data = token;
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
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new ApiResult<string>() { data = "/admin/login/" };
        }
    }
}