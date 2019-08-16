using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin.setting
{
    [AdminAuthorize(Roles = "Admin")]
    public class cardModifyModel : PageModel
    {
        public App.Data.Card Acard = null;
        public void OnGet(int id=0)
        {
            if (id > 0)
            {
                Acard = DbContext.Get().GetEntityDB<App.Data.Card>().GetById(id);
            }
        }
    }
}