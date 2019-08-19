using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin.user
{
    [AdminAuthorize(Roles = "Admin")]
    public class editModel : PageModel
    {
        public App.Data.UserInfo aUser = null;
        public void OnGet(int id = 0)
        {
            if (id > 0)
            {
                aUser = DbContext.Get().GetEntityDB<App.Data.UserInfo>().GetById(id);
            }
        }
    }
}