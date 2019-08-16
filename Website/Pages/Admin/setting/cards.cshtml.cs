using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin.setting
{
    [AdminAuthorize(Roles = "Admin")]
    public class cardsModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}