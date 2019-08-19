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



    }
}