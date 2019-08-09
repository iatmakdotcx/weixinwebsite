﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    [Authorize(Roles = "user")]
    public class DetailController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Save()
        {
            return View();
        }
    }
}