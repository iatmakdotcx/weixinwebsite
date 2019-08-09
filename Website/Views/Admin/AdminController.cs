using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MakC.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Website.Views.Admin
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
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

                token = "123456798";
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