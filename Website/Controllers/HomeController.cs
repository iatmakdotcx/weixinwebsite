using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Data;
using App.Extensions;
using MakC.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Website.Models;

namespace Website.Controllers
{
    [Authorize(Roles = "user")]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [Route("login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public ApiResult<string> postLogin(string tel,string pwd)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                var user = dbh.Db.Queryable<UserInfo>().First(ii => ii.tel == tel);
                if (user == null)
                {
                    apiRes.msg = "账号不正确";
                    return apiRes;
                }
                if (user.password != pwd)
                {
                    apiRes.msg = "密码不正确";
                    return apiRes;
                }

                setLogintoken(user);

                apiRes.ok = true;
                apiRes.data = "";
            }
            catch (Exception ex)
            {
                apiRes.ok = false;
                apiRes.msg = ex.Message;
                apiRes.data = "";
            }
            return apiRes;
        }
        private void setLogintoken(UserInfo user)
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var identity = new ClaimsPrincipal(
                 new ClaimsIdentity(new[]
                     {
                            new Claim(ClaimTypes.Sid, user.id.ToString()),
                            new Claim(ClaimTypes.Role,"user"),
                            new Claim(ClaimTypes.Name, user.nickname),
                            new Claim(ClaimTypes.MobilePhone, user.tel),
                            new Claim(ClaimTypes.Thumbprint, user.avatar),
                     //new Claim(ClaimTypes.DateOfBirth, user.birthday == null?"":user.birthday.Value.ToString("yyyy-MM-dd")),
                 }, CookieAuthenticationDefaults.AuthenticationScheme)
               );
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(7)) // 有效时间
                });
        }
        [Route("logout")]
        [AllowAnonymous]
        public IActionResult logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }
        [Route("reg")]
        [AllowAnonymous]
        public IActionResult reg()
        {
            return View();
        }
        [HttpPost("reg")]
        [AllowAnonymous]
        public ApiResult<string> regNewAccount(string tel, string pwd, string vcode)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                tel = tel.Replace(" ", "").Replace("+86", "");

#if !DEBUG
                if (vcode != MemoryCacheService.Default.GetCache<string>("vcode_" + tel))
                {
                    apiRes.msg = "手机验证码不正确";
                    return apiRes;
                }
#endif
                var dbh = DbContext.Get();
                UserInfo user = dbh.Db.Queryable<UserInfo>().First(ii => ii.tel == tel);
                if (user != null)
                {
                    //已存在 （重置密码
                    if (!string.IsNullOrEmpty(pwd))
                    {
                        user.password = pwd;
                        dbh.Db.Updateable(user).UpdateColumns(ii => ii.password).ExecuteCommand();
                    }
                }
                else
                {
                    user = new UserInfo();
                    user.createAt = DateTime.Now;
                    user.avatar = "/images/avatardef.jpg";
                    user.tel = tel;
                    user.password = pwd;
                    user.nickname = "手机用户" + tel.Substring(7);
                    user.id = dbh.Db.Insertable(user).ExecuteReturnIdentity();
                }

                setLogintoken(user);
                apiRes.ok = true;
                apiRes.data = "";
            }
            catch (Exception ex)
            {
                apiRes.ok = false;
                apiRes.msg = ex.Message;
                apiRes.data = "";
            }
            return apiRes;
        }
        [Route("agreement")]
        [AllowAnonymous]
        public IActionResult agreement(int id=1)
        {
            if (id == 2)
            {
                ViewData["agreementdata"] = SettingModel.getInstance().skincare_apc_agreement;
            }
            else
            {
                ViewData["agreementdata"] = SettingModel.getInstance().reg_agreement;             
            }
            return View();
        }

        [Route("profile")]
        public IActionResult profile()
        {
            var id = User.FindFirst(ClaimTypes.Sid).Value;
            UserInfo user = DbContext.Get().Db.Queryable<UserInfo>().InSingle(id);

            return View(user);
        }
        private static List<string> canUploadFileExt = new List<string>()
        {
            ".jpg",".gif",".bmp",".jpeg",".png"
        };
        [HttpPost("profile")]
        public async Task<IActionResult> SaveProfile(UserInfo Tmpuser )
        {
            var id = User.FindFirst(ClaimTypes.Sid).Value;
            var dbh = DbContext.Get();
            UserInfo user = dbh.Db.Queryable<UserInfo>().InSingle(id);
            user.nickname = Tmpuser.nickname;
            user.height = Tmpuser.height;
            user.weight = Tmpuser.weight;
            user.birthday = Tmpuser.birthday;
            user.address = Tmpuser.address;
            if (Request.Form.Files.Count > 0)
            {
                var formFile = Request.Form.Files[0];
                var ext = System.IO.Path.GetExtension(formFile.FileName).ToLower();
                if (formFile.Length > 0 && formFile.Length< 10*1024*1024 && canUploadFileExt.Contains(ext))
                {
                    using (var stream = formFile.OpenReadStream())
                    {
                        var bbf = new byte[stream.Length];
                        stream.Read(bbf, 0, (int)stream.Length);
                        user.avatar = "/images/userAcatar/" + Guid.NewGuid().ToString() + ext;
                        await System.IO.File.WriteAllBytesAsync(Environment.CurrentDirectory + "/wwwroot" + user.avatar, bbf);
                    }
                }
            }
            dbh.Db.Updateable(user).ExecuteCommand();

            return Redirect("/#/home/userinfo");
        }
        public IActionResult userinfo()
        {
            var id = User.FindFirst(ClaimTypes.Sid).Value;
            UserInfo user = DbContext.Get().Db.Queryable<UserInfo>().InSingle(id);

            return View(user);
        }

        public IActionResult yoga()
        {
     
            return View();
        }

        [AllowAnonymous]
        public IActionResult skincare()
        {
            var data = DbContext.Get().Db.Queryable<SkinCareClass>()
                .Where(ii => ii.enabled)
                .Select(ii => new { ii.id, ii.name, ii.avatar, ii.tags }).ToList();
            var jsonDat = JsonConvert.SerializeObject(data);
            return View((object)jsonDat);
        }
        public IActionResult mycards()
        {
     
            return View();
        }

        public IActionResult mycoupon()
        {
     
            return View();
        }




    }
}
