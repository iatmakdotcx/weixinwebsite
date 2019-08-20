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
        public IActionResult Save(int id)
        {
            var userid = User.FindFirst(ClaimTypes.Sid).Value.AsInt();
            var dbh = DbContext.Get();
            UserInfo user = dbh.Db.Queryable<UserInfo>().InSingle(userid);
            if (user == null)
            {
                return ShowErrorPage("登录已失效");
            }
            var data = dbh.Db.Queryable<YogaClass>().Select(ii=>new { ii.id,ii.disabled, ii.kyyzs ,ii.yysl ,ii.rdate }).Single(ii => ii.id == id);
            if (data == null || data.disabled)
            {
                return ShowErrorPage("课程不存在！");
            }
            if (data.kyyzs <= data.yysl)
            {
                return ShowErrorPage("当前课程预约已满！");
            }
            if (data.rdate < DateTime.Now.Date)
            {
                return ShowErrorPage("不能预约已过期的课程！");
            }

            if (dbh.Db.Queryable<YogaOrder>().Count(ii => ii.userId == userid && ii.classId == id) > 0)
            {
                return ShowErrorPage("已预约相同课程！");
            }
            YogaOrder order = new YogaOrder();
            order.tel = user.tel;
            order.create_at = DateTime.Now;
            order.classId = id;
            order.userId = userid;
            order.canceled = false;

            dbh.Db.Updateable<YogaClass>().SetColumns(ii => ii.yysl == ii.yysl + 1).Where(ii => ii.id == id).ExecuteCommand();
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