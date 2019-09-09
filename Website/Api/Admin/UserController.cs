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
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public ApiResult<ApiListObj<UserInfo>> list(int page, int limit, int a=0 ,string kw="")
        {
            ApiResult<ApiListObj<UserInfo>> alc = new ApiResult<ApiListObj<UserInfo>>();
            var dbh = DbContext.Get();
            alc.data = new ApiListObj<UserInfo>();
            if (a == 1)
            {
                alc.data.items = dbh.Db.Queryable<UserInfo>()
                    .WhereIF(!string.IsNullOrEmpty(kw) && "老师" != kw, ii => ii.nickname.Contains(kw) || ii.tel.Contains(kw))
                    .WhereIF("老师" == kw, ii => ii.isTeacher)
                    .ToList();
            }
            else
            {
                alc.data.items = dbh.Db.Queryable<UserInfo>()
                    .WhereIF(!string.IsNullOrEmpty(kw) && "老师" != kw, ii => ii.nickname.Contains(kw) || ii.tel.Contains(kw))
                    .WhereIF("老师" == kw, ii => ii.isTeacher)
                    .ToPageList(page, limit, ref alc.data.totalCnt);
            }
            alc.ok = true;
            return alc;
        }
        [HttpPost]
        public ApiResult<string> save(UserInfo acard)
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
                {
                    var rrUer = dbh.GetEntityDB<UserInfo>().GetById(acard.id);
                    if (rrUer == null)
                    {
                        apiRes.msg = "未找用户";
                        return apiRes;
                    }

                    rrUer.tel = acard.tel;
                    rrUer.nickname = acard.nickname;
                    rrUer.vipRebate = acard.vipRebate;
                    rrUer.address = acard.address;
                    rrUer.disabled = acard.disabled;
                    rrUer.height = acard.height;
                    rrUer.weight = acard.weight;
                    rrUer.description = acard.description;
                    rrUer.isTeacher = acard.isTeacher;
                    dbh.Db.Updateable(rrUer).ExecuteCommand();
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
}