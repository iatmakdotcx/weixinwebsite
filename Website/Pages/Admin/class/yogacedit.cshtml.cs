using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin
{
    public class yogaceditModel : PageModel
    {
        public App.Data.YogaClass data = null;
        public string rdate = "";
        public void OnGet(int id = 0, string rdate = "")
        {
            this.rdate = rdate;
            if (id > 0)
            {
                data = DbContext.Get().GetEntityDB<App.Data.YogaClass>().GetById(id);
            }
        }
    }
}