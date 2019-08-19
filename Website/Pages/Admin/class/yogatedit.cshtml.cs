﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin
{
    public class yogateditModel : PageModel
    {
        public App.Data.YogaClassTemplate data = null;
        public void OnGet(int id = 0)
        {
            if (id > 0)
            {
                data = DbContext.Get().GetEntityDB<App.Data.YogaClassTemplate>().GetById(id);
            }
        }
    }
}