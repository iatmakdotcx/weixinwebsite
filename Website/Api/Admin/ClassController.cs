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
    public class ClassController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public ApiResult<ApiListObj<SkinCareClass>> skclist(int page, int limit, string kw = "")
        {
            ApiResult<ApiListObj<SkinCareClass>> alc = new ApiResult<ApiListObj<SkinCareClass>>();
            var dbh = DbContext.Get();
            alc.data = new ApiListObj<SkinCareClass>();
            alc.data.items = dbh.Db.Queryable<SkinCareClass>()
                .WhereIF(!string.IsNullOrEmpty(kw), ii => ii.name.Contains(kw) || ii.tags.Contains(kw))
                .ToPageList(page, limit, ref alc.data.totalCnt);
            alc.ok = true;
            return alc;
        }
        [HttpPost]
        public ApiResult<string> saveskc(SkinCareClass acard)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                if (acard.id == 0)
                {
                    dbh.Db.Insertable(acard).ExecuteCommand();
                }
                else
                    dbh.Db.Updateable(acard).ExecuteCommand();

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

        public ApiResult<ApiListObj<YogaClassTemplate>> yogatemplatelist(int page, int limit, string kw = "")
        {
            ApiResult<ApiListObj<YogaClassTemplate>> alc = new ApiResult<ApiListObj<YogaClassTemplate>>();
            var dbh = DbContext.Get();
            alc.data = new ApiListObj<YogaClassTemplate>();
            alc.data.items = dbh.Db.Queryable<YogaClassTemplate>()
                .WhereIF(!string.IsNullOrEmpty(kw), ii => ii.name.Contains(kw) || ii.tags.Contains(kw))
                .ToPageList(page, limit, ref alc.data.totalCnt);
            alc.ok = true;
            return alc;
        }
       [HttpPost]
        public ApiResult<string> saveyogat(YogaClassTemplate acard)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                if (acard.id == 0)
                {
                    dbh.Db.Insertable(acard).ExecuteCommand();
                }
                else
                    dbh.Db.Updateable(acard).ExecuteCommand();

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
        public ApiResult<ApiListObj<YogaClass>> yogaclist(string rq = "")
        {
            DateTime dt;
            if (!DateTime.TryParse(rq, out dt))
            {
                dt = DateTime.Now.Date;
            }
            ApiResult<ApiListObj<YogaClass>> alc = new ApiResult<ApiListObj<YogaClass>>();
            var dbh = DbContext.Get();
            alc.data = new ApiListObj<YogaClass>();
            alc.data.items = dbh.Db.Queryable<YogaClass>()
                .Where(ii => ii.rdate == dt)
                .ToList();
            alc.ok = true;
            return alc;
        }

        [HttpPost]
        public ApiResult<string> saveyogac(YogaClass acard)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                if (acard.id == 0)
                {
                    dbh.Db.Insertable(acard).ExecuteCommand();
                }
                else
                    dbh.Db.Updateable(acard).IgnoreColumns(ii => ii.rdate).ExecuteCommand();

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
        public ApiResult<string> importclass(DateTime rdate, string ids)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                List<int> tureInt = new List<int>();
                foreach (var item in ids.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    int tmpint;
                    if (int.TryParse(item, out tmpint))
                    {
                        tureInt.Add(tmpint);
                    }
                }
                var dbh = DbContext.Get();
                var qrydata = dbh.Db.Queryable<YogaClassTemplate>().In(tureInt).ToList();
                foreach (var item in qrydata)
                {
                    dbh.Db.Insertable(new YogaClass()
                    {
                        name = item.name,
                        tags = item.tags,
                        avatar = item.avatar,
                        rdate = rdate,
                        rtimeRange = item.rtimeRange,
                        teacherid = item.teacherid,
                        teacher = item.teacher,
                        address = item.address,
                        description = item.description,
                        star = item.star,
                        kyyzs = item.kyyzs,
                        yysl = 0,
                    }).AddQueue();
                }
                dbh.Db.SaveQueuesAsync();
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
        public ApiResult<string> deleteYogaclass(int id)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                var yclass = dbh.GetEntityDB<YogaClass>().GetById(id);
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
    }
}