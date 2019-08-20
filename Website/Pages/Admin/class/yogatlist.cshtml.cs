using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin
{
    [AdminAuthorize(Roles = "Admin")]
    public class yogatlistModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}