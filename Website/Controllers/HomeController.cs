using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Data;
using App.Extensions;
using MakC.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Website.Models;

namespace Website.Controllers
{
    [Authorize(Roles = "user")]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult home()
        {
            return View();
        }
        [Route("login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public ApiResult<string> postLogin(string tel,string pwd)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                var user = dbh.Db.Queryable<UserInfo>().First(ii => ii.tel == tel);
                if (user == null)
                {
                    apiRes.msg = "账号不正确";
                    return apiRes;
                }
                if (user.password != mUtils.MD5Hash(pwd??""))
                {
                    apiRes.msg = "密码不正确";
                    return apiRes;
                }
                if (user.disabled)
                {
                    apiRes.msg = "账号已被停用";
                    return apiRes;
                }
                string wxid;
                if (string.IsNullOrEmpty(user.wxId) && !string.IsNullOrEmpty(wxid = HttpContext.Session.GetString("wxid")))
                {
                    //如果账号没有绑定微信，当前又是微信登录则自动绑定上
                    user.wxId = wxid;
                    dbh.Db.Updateable(user).UpdateColumns(ii => ii.wxId).ExecuteCommand();
                }
                setLogintoken(user);

                apiRes.ok = true;
                apiRes.data = "";
            }
            catch (Exception ex)
            {
                apiRes.ok = false;
                apiRes.msg = ex.Message;
                apiRes.data = "";
            }
            return apiRes;
        }
        [Route("wx")]
        [AllowAnonymous]
        public IActionResult wxLogin(string code="")
        {
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var wxid = MakC.Common.Auth.Auth_WeiXin.Get_openid(code);
                    if (!string.IsNullOrEmpty(wxid))
                    {
                        HttpContext.Session.SetString("wxid", wxid);

                        var dbh = DbContext.Get();
                        var usinfo = dbh.Db.Queryable<UserInfo>().First(ii => ii.wxId == wxid);
                        if (usinfo != null && usinfo.disabled)
                        {
                            //已绑定微信，自动登录
                            setLogintoken(usinfo);
                        }
                    }
                }
                catch (Exception)
                {}
            }
            return Redirect("/");
        }
        private void setLogintoken(UserInfo user)
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var identity = new ClaimsPrincipal(
                 new ClaimsIdentity(new[]
                     {
                            new Claim(ClaimTypes.Sid, user.id.ToString()),
                            new Claim(ClaimTypes.Role,"user"),
                            new Claim(ClaimTypes.Name, user.nickname),
                            new Claim(ClaimTypes.MobilePhone, user.tel),
                            new Claim(ClaimTypes.Thumbprint, user.avatar),
                     //new Claim(ClaimTypes.DateOfBirth, user.birthday == null?"":user.birthday.Value.ToString("yyyy-MM-dd")),
                 }, CookieAuthenticationDefaults.AuthenticationScheme)
               );
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(7)) // 有效时间
                });
        }
        [Route("logout")]
        [AllowAnonymous]
        public IActionResult logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }
        [Route("reg")]
        [AllowAnonymous]
        public IActionResult reg()
        {
            return View();
        }
        [HttpPost("reg")]
        [AllowAnonymous]
        public ApiResult<string> regNewAccount(string tel, string vcode, string pwd = "")
        {
            var apiRes = new ApiResult<string>();
            try
            {
                tel = tel.Replace(" ", "").Replace("+86", "");
                if (vcode != MemoryCacheService.Default.GetCache<string>("vcode_" + tel))
                {
                    apiRes.msg = "手机验证码不正确";
                    return apiRes;
                }
                var dbh = DbContext.Get();
                UserInfo user = dbh.Db.Queryable<UserInfo>().First(ii => ii.tel == tel);
                if (user != null)
                {
                    //已存在 （重置密码
                    if (!string.IsNullOrEmpty(pwd))
                    {
                        user.password = pwd;
                        dbh.Db.Updateable(user).UpdateColumns(ii => ii.password).ExecuteCommand();
                    }
                    if (user.disabled)
                    {
                        apiRes.msg = "账号已被停用";
                        return apiRes;
                    }
                }
                else
                {
                    user = new UserInfo();
                    user.createAt = DateTime.Now;
                    user.tel = tel;
                    user.password = pwd;
                    user.nickname = "手机用户" + tel.Substring(7);
                    user.wxId = HttpContext.Session.GetString("wxid");
                    user.id = dbh.Db.Insertable(user).ExecuteReturnIdentity();
                }
                if (string.IsNullOrEmpty(user.avatar))
                {
                    user.avatar = "/images/avatardef.jpg";
                }
                setLogintoken(user);
                apiRes.ok = true;
                apiRes.data = "";
            }
            catch (Exception ex)
            {
                apiRes.ok = false;
                apiRes.msg = ex.Message;
                apiRes.data = "";
            }
            return apiRes;
        }
        [Route("agreement")]
        [AllowAnonymous]
        public IActionResult agreement(int id=1)
        {
            if (id == 2)
            {
                ViewData["agreementdata"] = SettingModel.getInstance().skincare_apc_agreement;
            }
            else
            {
                ViewData["agreementdata"] = SettingModel.getInstance().reg_agreement;             
            }
            return View();
        }

        [Route("profile")]
        public IActionResult profile()
        {
            var id = User.FindFirst(ClaimTypes.Sid).Value;
            UserInfo user = DbContext.Get().Db.Queryable<UserInfo>().InSingle(id);

            return View(user);
        }
        private static List<string> canUploadFileExt = new List<string>()
        {
            ".jpg",".gif",".bmp",".jpeg",".png"
        };
        [HttpPost("profile")]
        public async Task<IActionResult> SaveProfile(UserInfo Tmpuser )
        {
            var id = User.FindFirst(ClaimTypes.Sid).Value;
            var dbh = DbContext.Get();
            UserInfo user = dbh.Db.Queryable<UserInfo>().InSingle(id);
            user.nickname = Tmpuser.nickname;
            user.height = Tmpuser.height;
            user.weight = Tmpuser.weight;
            user.birthday = Tmpuser.birthday;
            user.address = Tmpuser.address;
            if (Request.Form.Files.Count > 0)
            {
                var formFile = Request.Form.Files[0];
                var ext = System.IO.Path.GetExtension(formFile.FileName).ToLower();
                if (formFile.Length > 0 && formFile.Length< 10*1024*1024 && canUploadFileExt.Contains(ext))
                {
                    using (var stream = formFile.OpenReadStream())
                    {
                        var bbf = new byte[stream.Length];
                        stream.Read(bbf, 0, (int)stream.Length);
                        user.avatar = "/images/userAcatar/" + Guid.NewGuid().ToString() + ext;
                        await System.IO.File.WriteAllBytesAsync(Environment.CurrentDirectory + "/wwwroot" + user.avatar, bbf);
                    }
                }
            }
            dbh.Db.Updateable(user).ExecuteCommand();

            return Redirect("/#/home/userinfo");
        }
        public IActionResult userinfo()
        {
            var id = User.FindFirst(ClaimTypes.Sid).Value;
            UserInfo user = DbContext.Get().Db.Queryable<UserInfo>().InSingle(id);

            return View(user);
        }

        [AllowAnonymous]
        public IActionResult yoga()
        {
            var ydata = yoga_data(DateTime.Now);
            if (!ydata.ok)
            {
                //error
            }
            TempData["data"] = JsonConvert.SerializeObject(ydata.data);

            return View();
        }
        [AllowAnonymous]
        public ApiResult<List<YogaClass>> yoga_data(DateTime rq)
        {
            ApiResult<List<YogaClass>> result = new ApiResult<List<YogaClass>>();
            var dbh = DbContext.Get();
            var lstdata = dbh.Db.Queryable<YogaClass>()
                .Where(ii => ii.rdate == rq.Date && !ii.disabled)
                .OrderBy(ii => ii.rtimeRange)
                .IgnoreColumns(ii=> new { ii.description})
                //.Select(ii => new { ii.id, ii.name, ii.RtimeRange, ii.star, ii.tags, ii.avatar, ii.teacher })
                .ToList();

            result.data = lstdata;
            result.ok = true;

            return result;
        }

        [AllowAnonymous]
        public ApiResult<List<PTSchedule>> pt_data(DateTime rq)
        {
            ApiResult<List<PTSchedule>> result = new ApiResult<List<PTSchedule>>();
            var dbh = DbContext.Get();
            var lstdata = dbh.Db.Queryable<PTSchedule>()
                .Where(ii => ii.rdate == rq.Date)
                .OrderBy(ii => ii.rtime)
                .IgnoreColumns(ii => new { ii.description })
                //.Select(ii => new { ii.id, ii.name, ii.RtimeRange, ii.star, ii.tags, ii.avatar, ii.teacher })
                .ToList();

            result.data = lstdata;
            result.ok = true;

            return result;
        }



        [AllowAnonymous]
        public IActionResult skincare()
        {
            var data = DbContext.Get().Db.Queryable<SkinCareClass>()
                .Where(ii => !ii.disabled)
                .Select(ii => new { ii.id, ii.name, ii.avatar, ii.tags }).ToList();
            var jsonDat = JsonConvert.SerializeObject(data);
            return View((object)jsonDat);
        }
        public IActionResult mycards()
        {
            var userid = User.FindFirst(ClaimTypes.Sid).Value.AsInt();
            var dbh = DbContext.Get();
            var qryd = dbh.Db.Queryable<UserCard, Card>((t1, t2) => new object[] { SqlSugar.JoinType.Left, t1.cardsId == t2.id })
                .Where((t1, t2) => t1.userid == userid && t1.cardexpiryDate >= DateTime.Now.Date && t1.usedCnt<t1.canUseCnt)
                .Select((t1, t2) => new MyCardsModel() {
                    name = t1.cardname,
                    img = t2.img,
                    type = t1.cardtype,
                    description = t2.description,
                    cornerMark = t2.cornerMark,
                    expiryDate = t1.cardexpiryDate,
                    canUseCnt = t1.canUseCnt,
                    usedCnt = t1.usedCnt })
                .ToList();

            return View(qryd);
        } public IActionResult mytickets()
        {
            var userid = User.FindFirst(ClaimTypes.Sid).Value.AsInt();
            var dbh = DbContext.Get();
            var qryd = dbh.Db.Queryable<UserTicket, Ticket>((t1, t2) => new object[] { SqlSugar.JoinType.Left, t1.ticketid == t2.id })
                .Where((t1, t2) => t1.userid == userid && !t1.deleted && t1.expiryDate >= DateTime.Now.Date && t1.useOrder == null)
                .Select((t1, t2) => new MyticketsModel()
                {
                    name = t2.name,
                    type = t2.type,
                    expiryDate = t1.expiryDate,
                    value = t2.value,
                    threshold = t2.threshold
                })
                .ToList();

            return View(qryd);
        }

        public IActionResult myygorder()
        {
            var userid = User.FindFirst(ClaimTypes.Sid).Value.AsInt();
            var dbh = DbContext.Get();
            var dd = DateTime.Now.Date.AddDays(-365);

            var qryd = dbh.Db.Queryable<YogaOrder, YogaClass>((t1, t2) => new object[] { SqlSugar.JoinType.Left, t1.classId == t2.id })
                .Where((t1, t2) => t1.userId == userid && t2.rdate > dd)
                .Select((t1, t2) => new { t1.id, t2.rdate, t2.rtimeRange, className = t2.name, t2.teacher, t2.address, t1.canceled })
                .MergeTable()
                .OrderBy(ii => ii.rdate, SqlSugar.OrderByType.Desc).ToList();
            var d1 = new JArray();
            var d2 = new JArray();
            var d3 = new JArray();
            var tdv = DateTime.Now.Date;
            foreach (var item in qryd)
            {
                if (item.canceled)
                {
                    d3.Add(new JObject(
                        new JProperty("id", item.id),
                        new JProperty("rdate", item.rdate.ToString("yyyy-MM-dd")),
                        new JProperty("className", item.className),
                        new JProperty("rtimeRange", item.rtimeRange),
                        new JProperty("teacher", item.teacher),
                        new JProperty("address", item.address)
                        ));
                }
                else if (item.rdate < tdv)
                {
                    d2.Add(new JObject(
                        new JProperty("id", item.id),
                        new JProperty("rdate", item.rdate.ToString("yyyy-MM-dd")),
                        new JProperty("className", item.className),
                        new JProperty("rtimeRange", item.rtimeRange),
                        new JProperty("teacher", item.teacher),
                        new JProperty("address", item.address)
                        ));
                }
                else
                {
                    d1.Add(new JObject(
                        new JProperty("id", item.id),
                        new JProperty("rdate", item.rdate.ToString("yyyy-MM-dd")),
                        new JProperty("className", item.className),
                        new JProperty("rtimeRange", item.rtimeRange),
                        new JProperty("teacher", item.teacher),
                        new JProperty("address", item.address)
                        ));
                }
            }
            var tmpAllData = new JObject(new JProperty("d1", d1), new JProperty("d2", d2), new JProperty("d3", d3));
            TempData["listdata"] = tmpAllData.ToString(Formatting.None);
            return View();
        }
        public IActionResult myscorder()
        {
            var userid = User.FindFirst(ClaimTypes.Sid).Value.AsInt();
            var dbh = DbContext.Get();
            var qryd = dbh.Db.Queryable<SkinCareOrder, SkinCareClass>((t1, t2) => new object[] { SqlSugar.JoinType.Left, t1.ClassId == t2.id })
                .Where((t1, t2) => t1.UserId == userid && t1.reserveDate > DateTime.Now.Date.AddDays(-365))
                .Select((t1, t2) => new { t1.id, t1.reserveDate, className = t2.name, t2.teacher, t2.address, t1.canceled })
                .OrderBy(t1 => t1.reserveDate, SqlSugar.OrderByType.Desc).ToList();
            var d1 = new JArray();
            var d2 = new JArray();
            var d3 = new JArray();
            var tdv =DateTime.Now.Date;
            foreach (var item in qryd)
            {
                if (item.canceled)
                {
                    d3.Add(new JObject(
                        new JProperty("id", item.id),
                        new JProperty("reserveDate", item.reserveDate.ToString("yyyy-MM-dd")),
                        new JProperty("className", item.className),
                        new JProperty("teacher", item.teacher),
                        new JProperty("address", item.address)
                        ));
                }
                else if (item.reserveDate< tdv)
                {
                    d2.Add(new JObject(
                        new JProperty("id", item.id),
                        new JProperty("reserveDate", item.reserveDate.ToString("yyyy-MM-dd")),
                        new JProperty("className", item.className),
                        new JProperty("teacher", item.teacher),
                        new JProperty("address", item.address)
                        ));
                }
                else
                {
                    d1.Add(new JObject(
                        new JProperty("id", item.id),
                        new JProperty("reserveDate", item.reserveDate.ToString("yyyy-MM-dd")),
                        new JProperty("className", item.className),
                        new JProperty("teacher", item.teacher),
                        new JProperty("address", item.address)
                        ));
                }
            }
            var tmpAllData = new JObject(new JProperty("d1", d1), new JProperty("d2", d2), new JProperty("d3", d3));
            TempData["listdata"]= tmpAllData.ToString(Formatting.None);
            return View();
        }
        public IActionResult myptorder()
        {
            var userid = User.FindFirst(ClaimTypes.Sid).Value.AsInt();
            var dbh = DbContext.Get();
            var qryd = dbh.Db.Queryable<PtOrder, PTSchedule>((t1, t2) => t1.ptId == t2.id)
                .Where((t1, t2) => t1.userId == userid && t2.rdate > DateTime.Now.Date.AddDays(-365))
                .Select((t1, t2) => new { t1.id, t2.rdate, className = t2.name, t2.rlen, t2.username, t2.rtime, t1.canceled })
                .MergeTable()
                .OrderBy(t1 => t1.rdate, SqlSugar.OrderByType.Desc).ToList();
            var d1 = new JArray();
            var d2 = new JArray();
            var d3 = new JArray();
            var tdv =DateTime.Now.Date;
            foreach (var item in qryd)
            {
                if (item.canceled)
                {
                    d3.Add(new JObject(
                        new JProperty("id", item.id),
                        new JProperty("rdate", item.rdate.ToString("yyyy-MM-dd")),
                        new JProperty("teacher", item.username),
                        new JProperty("rtime", item.rtime),
                        new JProperty("rlen", item.rlen)
                        ));
                }
                else if (item.rdate < tdv)
                {
                    d2.Add(new JObject(
                        new JProperty("id", item.id),
                        new JProperty("rdate", item.rdate.ToString("yyyy-MM-dd")),
                        new JProperty("teacher", item.username),
                        new JProperty("rtime", item.rtime),
                        new JProperty("rlen", item.rlen)
                        ));
                }
                else
                {
                    d1.Add(new JObject(
                        new JProperty("id", item.id),
                        new JProperty("rdate", item.rdate.ToString("yyyy-MM-dd")),
                        new JProperty("teacher", item.username),
                        new JProperty("rtime", item.rtime),
                        new JProperty("rlen", item.rlen)
                        ));
                }
            }
            var tmpAllData = new JObject(new JProperty("d1", d1), new JProperty("d2", d2), new JProperty("d3", d3));
            TempData["listdata"]= tmpAllData.ToString(Formatting.None);
            return View();
        }

        public IActionResult sign()
        {           
            var data = sign_data(DateTime.Now.Date);
            if (!data.ok)
            {
                return ShowErrorPage(data.msg);
            }
            TempData["data"] = JsonConvert.SerializeObject(data.data);
            return View();
        }
        public ApiResult<List<YogaClass>> sign_data(DateTime rq)
        {
            ApiResult<List<YogaClass>> result = new ApiResult<List<YogaClass>>();
            var id = User.FindFirst(ClaimTypes.Sid).Value.AsInt();
            var dbh = DbContext.Get();
            UserInfo user = dbh.Db.Queryable<UserInfo>().InSingle(id);
            if (user == null || !user.isTeacher)
            {
                result.msg = "无法使用此功能";
                return result;
            }
            result.data = dbh.Db.Queryable<YogaClass>().Where(ii => ii.teacherid == id && ii.rdate == rq).ToList(); 
            result.ok = true;
            return result;
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
        public IActionResult toc()
        {
            return View();
        }
        [HttpPost]
        public IActionResult tocresult(DateTime qrydate, TocResultModel model)
        {
            model.qrydate = qrydate;
            if (model.type == "1")
            {
                //私教
            }
            else
            { //瑜伽

                var dbh = DbContext.Get();
                model.yoga = dbh.Db.Queryable<YogaClass>().Where(ii => ii.rdate == model.qrydate).ToList();
            }
            return View(model);
        }
        public IActionResult tou_yoga(int id)
        {
            var dbh = DbContext.Get();
            Tou_yogaModel model = new Tou_yogaModel();
            model.data = dbh.Db.Queryable<YogaClass>().First(ii => ii.id == id);

            dbh.Db.Queryable<YogaClass>().First(ii => ii.id == id);
            //查询7天内预约了相同课程的人
            model.users = dbh.Db.Ado.SqlQuery<dynamic>(@"select distinct a.id,a.tel,a.nickname name FROM users a join yogaorder b on a.id=b.userid
where classid in (select id from yogaclass where Rdate >= '" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd")+"' and name = '"+ model.data.name+ "') limit 0,30");

            return View(model);
        }
        [HttpPost]
        public ApiResult<string> tousave(int classid, string ids)
        {
            ApiResult<string> result = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                var aids = dbh.Db.Queryable<UserInfo>().Select(ii => new { ii.id, ii.tel }).In("id", mUtils.idsToList(ids)).ToList();

                foreach (var item in aids)
                {
                    if (dbh.GetEntityDB<YogaOrder>().Count(ii => ii.classId == classid && ii.userId == item.id) == 0)
                    {
                        YogaOrder yo = new YogaOrder();
                        yo.create_at = DateTime.Now;
                        yo.classId=classid;
                        yo.userId = item.id;
                        yo.tel = item.tel;
                        yo.canceled = false;
                        dbh.Db.Insertable(yo).ExecuteCommand();
                    }
                }
                result.ok = true;
            }
            catch (Exception ex)
            {
                result.ok = false;
                result.msg = ex.Message;
            }
            return result;
        }
        public IActionResult tocsave()
        {
            return View();
        }
    }
}
