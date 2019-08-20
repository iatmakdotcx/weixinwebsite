using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Data;
using MakC.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Models;

namespace Website.Controllers
{
    [Authorize(Roles = "user")]
    public class DetailController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index(int id)
        {
            var dbh = DbContext.Get();
            var data = dbh.GetEntityDB<YogaClass>().GetById(id);
            if (data == null || data.disabled)
            {
                return ShowErrorPage("课程不存在！");
            }
            detailModel mdl = new detailModel();
            mdl.data = data;
            Claim idobj;
            if (User != null && (idobj = User.FindFirst(ClaimTypes.Sid)) != null)
            {
                var userid = idobj.Value.AsInt();
                if (userid > 0)
                {
                    mdl.ordered = dbh.Db.Queryable<YogaOrder>().Count(ii => ii.userId == userid && ii.classId == id) > 0;
                }
            }
            return View(mdl);
        }
        public IActionResult Save(int id, string tel)
        {
            var userid = User.FindFirst(ClaimTypes.Sid).Value.AsInt();
            var dbh = DbContext.Get();
            UserInfo user = dbh.Db.Queryable<UserInfo>().InSingle(userid);
            if (user == null)
            {
                return ShowErrorPage("登录已失效");
            }
            var data = dbh.GetEntityDB<YogaClass>().GetById(id);
            if (data == null || data.disabled)
            {
                return ShowErrorPage("课程不存在！");
            }

            YogaOrder order = dbh.Db.Queryable<YogaOrder>().First(ii => ii.userId == userid && ii.classId == id);
            if (order != null)
            {
                return ShowErrorPage("已预约相同课程！");
            }
            order = new YogaOrder();
            order.tel = tel;
            order.create_at = DateTime.Now;
            order.classId = id;
            order.userId = userid;
            order.canceled = false;

            dbh.Db.Insertable(order).ExecuteCommand();
            return View();
        }
        private IActionResult ShowErrorPage(string msg, string description = "")
        {
            ErrorViewModel erv = new ErrorViewModel();
            erv.Msg = msg;
            erv.Description = description;
            erv.Url = HttpContext.Request.Path;
            //erv.Referer = HttpContext.Request.Headers["Referer"];
            return View("Error", erv);
        }
    }
}