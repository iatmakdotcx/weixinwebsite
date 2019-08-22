using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.user
{
    [AdminAuthorize(Roles = "Admin")]
    public class userSelectModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}