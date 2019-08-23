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
        public ApiResult<ApiListObj<PTSchedule>> clist(string rq = "")
        {
            DateTime dt;
            if (!DateTime.TryParse(rq, out dt))
            {
                dt = DateTime.Now.Date;
            }
            ApiResult<ApiListObj<PTSchedule>> alc = new ApiResult<ApiListObj<PTSchedule>>();
            var dbh = DbContext.Get();
            alc.data = new ApiListObj<PTSchedule>();
            alc.data.items = dbh.Db.Queryable<PTSchedule>()
                .Where(ii => ii.rdate == dt)
                .ToList();
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
        public ApiResult<string> importptt(DateTime rdate, string ids)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var insertSql = @"INSERT INTO ptschedule(`name`,userId,username,avatar,tags,rdate,rtime,rlen,description,kyyzs,yysl)
select `name`,userId,users.nickname,users.avatar ,tags,'"+ rdate.ToString("yyyy-MM-dd") + @"',rtime,rlen,pttemplate.description,kyyzs,0
from pttemplate join users on pttemplate.userId=users.id
where pttemplate.id in(" + mUtils.checkids(ids) + ")";

                var dbh = DbContext.Get();
                dbh.Db.Ado.ExecuteCommand(insertSql);

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
        public ApiResult<string> delete(int id)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                var yclass = dbh.GetEntityDB<PTSchedule>().GetById(id);
                if (yclass != null)
                {
                    if (yclass.rdate < DateTime.Now.Date)
                    {
                        apiRes.msg = "不能删除历史课程";
                        return apiRes;
                    }
                    if (yclass.yysl > 0)
                    {
                        apiRes.msg = "课程已被预约！";
                        return apiRes;
                    }
                    dbh.Db.Deleteable(yclass).ExecuteCommand();
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
        public ApiResult<string> savepts(PTSchedule obj)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                if (obj.id == 0)
                {
                    dbh.Db.Insertable(obj).ExecuteCommand();
                }
                else
                    dbh.Db.Updateable(obj).IgnoreColumns(ii => ii.rdate).ExecuteCommand();

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