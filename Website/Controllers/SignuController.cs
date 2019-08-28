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
    public class SignuController : Controller
    {
        public IActionResult Index(int id)
        {
            var dbh = DbContext.Get();
            var data = dbh.GetEntityDB<YogaClass>().GetById(id);
            if (data == null || data.disabled)
            {
                return ShowErrorPage("课程不存在！");
            }
            signListModel mdl = new signListModel();
            mdl.data = data;
            mdl.listitems = dbh.Db.Queryable<YogaOrder, UserInfo>((t1, t2) => t1.userId == t2.id)
                .Where((t1, t2) => t1.classId == id)
                .Select((t1, t2) => new signListitem()
                {
                    id = t1.id,
                    classid = t1.classId,
                    userid= t1.userId,
                    tel=t2.tel,
                    name=t2.nickname,
                    canceled=t1.canceled
                })
                .ToList();
            return View(mdl);
        }
        public ApiResult<string> Save(int id, string idxs)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                idxs = "," + idxs + ",";
                var userid = User.FindFirst(ClaimTypes.Sid).Value.AsInt();
                var dbh = DbContext.Get();
                var data = dbh.GetEntityDB<YogaClass>().GetById(id);
                if (data == null || data.disabled)
                {
                    apiRes.msg = "课程不存在";
                    return apiRes;
                }
                if (data.rdate < DateTime.Now.Date)
                {
                    apiRes.msg = "课程已过期";
                    return apiRes;
                }
                var qdrs = 0;
                var ords = dbh.Db.Queryable<YogaOrder>().Where(ii => ii.classId == id).ToList();
                foreach (var item in ords)
                {
                    if(idxs.IndexOf(","+ item.id + ",")>-1)
                    {
                        item.canceled = true;
                        item.cancelTime= DateTime.Now;
                        item.cancelMsg = "未签到";
                        dbh.Db.Updateable(item).ExecuteCommand();
                    }
                    else
                    {
                        item.canceled = false;
                        item.cancelTime = null;
                        item.cancelUser = null;
                        item.cancelMsg = null;
                        dbh.Db.Updateable(item).ExecuteCommand();
                        qdrs++;
                    }
                }
                data.yysl = ords.Count;
                data.qdsl = qdrs;
                dbh.Db.Updateable(data).ExecuteCommand();
                apiRes.ok = true;
            }
            catch (Exception ex)
            {
                apiRes.ok = false;
                apiRes.msg = ex.Message;
                apiRes.data = "";
            }
            return apiRes;
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
    public class signListitem
    {
        public int id { get; set; }
        public int classid { get; set; }
        public int userid { get; set; }
        public string tel { get; set; }
        public string name { get; set; }
        public bool canceled { get; set; }
    }

}