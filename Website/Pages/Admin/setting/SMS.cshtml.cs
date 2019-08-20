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
    public class SMSModel : PageModel
    {
        public void OnGet()
        {
        }

    }
}