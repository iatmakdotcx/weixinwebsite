using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MakC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin.setting
{
    public class weixinModel : PageModel
    {
        public string Appid;
        public string Key;

        public void OnGet()
        {
            var data = DbContext.Get().GetEntityDB<MakC.Data.Model.Setting>().GetList(ii => ii.key == "weixin_appid" || ii.key == "weixin_key");
            if (data[0].key)
            {
                
            }
        }
        public void OnPost()
        {

        }
    }
}