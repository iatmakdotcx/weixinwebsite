using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Extensions;
using MakC.Common;
using App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Website.Api.Admin
{
    [Route("api/admin/[controller]/[action]")]
    //[ApiController]
    [AdminAuthorize(Roles = "Admin")]
    public class SettingController : ControllerBase
    {
        public string test()
        {
            return "SettingController";
        }
        [HttpPost]
        public ApiResult<string> weixin(string appid, string key)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                ConfigHelper.Configuration["Weixin:appID"] = appid;
                ConfigHelper.Configuration["Weixin:appsecret"] = key;
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
        public ApiResult<string> register(string open, string agreement)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                int optc = dbh.Db.Updateable<Setting>().SetColumns(ii => ii.value == open).Where(ii => ii.key == "reg_open").ExecuteCommand();
                if (optc == 0)
                {
                    dbh.Db.Insertable(new Setting() { key = "reg_open", value = open }).ExecuteCommand();
                }
                agreement = agreement.Replace("\n", "<br />");
                optc = dbh.Db.Updateable<Setting>().SetColumns(ii => ii.value == agreement).Where(ii => ii.key == "reg_agreement").ExecuteCommand();
                if (optc == 0)
                {
                    dbh.Db.Insertable(new Setting() { key = "reg_agreement", value = agreement }).ExecuteCommand();
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