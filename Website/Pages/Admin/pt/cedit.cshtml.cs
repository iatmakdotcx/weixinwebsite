using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin
{
    [AdminAuthorize(Roles = "Admin")]
    public class ceditModel : PageModel
    {
        public App.Data.PTSchedule data = null;
        public void OnGet(int id = 0)
        {
            if (id > 0)
            {
                var dbh = DbContext.Get();
                data = dbh.GetEntityDB<PTSchedule>().GetById(id);                
            }
        }
    }
}