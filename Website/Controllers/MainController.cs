using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MainController : Controller
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
        public IActionResult postLogin()
        {
            var identity = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                    {
                            new Claim(ClaimTypes.Sid,"0123456789"),
                            new Claim(ClaimTypes.Role,"Admin"),
                            new Claim(ClaimTypes.Name,"admin's name"),
                            new Claim(ClaimTypes.WindowsAccountName,"admin"),
                            new Claim(ClaimTypes.UserData,"user.UpLoginDate.ToString()")
                    }, CookieAuthenticationDefaults.AuthenticationScheme)
            );
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(7)) // 有效时间
                });
            return View();
        }
        [Route("logout")]
        [AllowAnonymous]
        public IActionResult logout()
        {

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

    }
}