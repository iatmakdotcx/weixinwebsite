using System;
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
        [AllowAnonymous]
        public IActionResult Index(int id)
        {
            //var data = DbContext.Get().Db.Queryable<SkinCareClass>()
            //    .Where(ii => ii.enabled)
            //    .Select(ii => new { ii.id, ii.name, ii.avatar, ii.tags }).ToList();
            return View();
        }
        public IActionResult Save()
        {
            return View();
        }
    }
}