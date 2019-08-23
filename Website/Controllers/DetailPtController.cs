using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Data;
using MakC.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Website.Models;

namespace Website.Controllers
{
    [Authorize(Roles = "user")]
    public class DetailPtController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index(int id)
        {
            var dbh = DbContext.Get();
            var data = dbh.Db.Queryable<PTSchedule>().InSingle(id);
            detailPtModel model = new detailPtModel();
            model.data = data;            
            return View(model);
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
            var pts = dbh.Db.Queryable<PTSchedule>().First(ii => ii.id == id);
            if (pts == null)
            {
                return ShowErrorPage("无效id");
            }
            if (pts.kyyzs <= pts.yysl)
            {
                return ShowErrorPage("私教已约满");
            }
          

            if (dbh.Db.Queryable<PtOrder>().Count(ii => ii.userId == userid && ii.ptId == id)>0)
            {                
                return ShowErrorPage("已预约过相同日期");
            }
            PtOrder sco = new PtOrder();
            sco.create_at = DateTime.Now;
            sco.tel = user.tel;
            sco.ptId = id;
            sco.userId = userid;
            sco.canceled = false;
            dbh.Db.Insertable(sco).ExecuteCommand();
            pts.yysl++;
            dbh.Db.Updateable(pts).UpdateColumns(ii => ii.yysl).ExecuteCommand();
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