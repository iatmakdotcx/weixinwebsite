using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Extensions;
using MakC.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin.setting
{
    [AdminAuthorize(Roles = "Admin")]
    public class weixinModel : PageModel
    {
        public string Appid;
        public string Key;

        public void OnGet()
        {
            Appid = ConfigHelper.Configuration["Weixin:appID"];
            Key = ConfigHelper.Configuration["Weixin:appsecret"];
        }

    }
}