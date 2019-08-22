using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Extensions;
using MakC.Common;
using Microsoft.AspNetCore.Mvc;

namespace Website.Api.Admin
{
    [Produces("application/json")]
    [Route("api/admin/[controller]/[action]")]
    [AdminAuthorize(Roles = "Admin")]
    public class PtController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public ApiResult<ApiListObj<PttlistItem>> list(int page, int limit, string kw="")
        {
            ApiResult<ApiListObj<PttlistItem>> alc = new ApiResult<ApiListObj<PttlistItem>>();
            var dbh = DbContext.Get();
            alc.data = new ApiListObj<PttlistItem>();
            alc.data.items = dbh.Db.Queryable<PTTemplate,UserInfo>((t1,t2)=>t1.userId==t2.id)
                .Select((t1,t2)=>new PttlistItem(){
                    id=t1.id,
                    name = t1.name,
                    tags=t1.tags,
                    userid= t1.userId,
                    avatar=t2.avatar,
                    nickname=t2.nickname,
                    rtime= t1.rtime,
                    rlen=t1.rlen }).MergeTable()
                .WhereIF(!string.IsNullOrEmpty(kw), ii => ii.nickname.Contains(kw) || ii.name.Contains(kw))
                .ToPageList(page, limit, ref alc.data.totalCnt);
            alc.ok = true;
            return alc;
        }
        [HttpPost]
        public ApiResult<string> saveptt(PTTemplate ptt)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                if (ptt.id == 0)
                {
                    dbh.Db.Insertable(ptt).ExecuteCommand();
                }
                else
                    dbh.Db.Updateable(ptt).ExecuteCommand();

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
        [HttpPost]
        public ApiResult<string> resetPassword(int id, string tel, string newpwd)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                var optCnt = dbh.Db.Updateable<UserInfo>()
                    .SetColumns(ii => ii.password == newpwd)
                    .Where(ii => ii.id == id && ii.tel == tel).ExecuteCommand();
                if (optCnt != 1)
                {
                    apiRes.msg = "设置用户密码失败";
                    return apiRes;
                }
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
        [HttpPost]
        public ApiResult<string> addMulti(string data)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var telArr = data.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                if (telArr.Length > 0)
                {
                    var dbh = DbContext.Get();
                    foreach (var item in telArr)
                    {
                        string tel = item.Trim();
                        if (tel.Length >= 11 && dbh.Db.Queryable<UserInfo>().Count(ii => ii.tel == tel) == 0)
                        {
                            dbh.Db.Insertable(new UserInfo() { tel = tel, nickname = "用户" + tel.Substring(7), balance = 0, createAt = DateTime.Now }).ExecuteCommand();
                        }
                    }                    
                }
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
    }
    public class PttlistItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string tags { get; set; }
        public string avatar { get; set; }
        public string rtime { get; set; }
        public int rlen { get; set; }
        public int userid { get; set; }
        public string nickname { get; set; }

    }
}