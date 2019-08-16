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
        private static List<string> canUploadFileExt = new List<string>()
        {
            ".jpg",".gif",".bmp",".jpeg",".png"
        };
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

        public ApiResult<ApiListObj<Card>> getcards(int page, int limit)
        {
            ApiResult<ApiListObj<Card>> alc = new ApiResult<ApiListObj<Card>>();
            var dbh = DbContext.Get();
            alc.data = new ApiListObj<Card>();
            alc.data.totalCnt = dbh.Db.Queryable<Card>().Count();
            alc.data.items = dbh.Db.Queryable<Card>().ToList();
            alc.ok = true;
            return alc;
        }
        [HttpPost]
        public ApiResult<string> saveCard(Card acard)
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
        [HttpPost]
        public async Task<ApiResult<string>> cardimg()
        {
            var apiRes = new ApiResult<string>();
            try
            {
                apiRes.data = "";
                if (Request.Form.Files.Count > 0)
                {
                    var formFile = Request.Form.Files[0];
                    var ext = System.IO.Path.GetExtension(formFile.FileName).ToLower();
                    if (formFile.Length > 0 && formFile.Length < 10 * 1024 * 1024 && canUploadFileExt.Contains(ext))
                    {
                        using (var stream = formFile.OpenReadStream())
                        {
                            var bbf = new byte[stream.Length];
                            stream.Read(bbf, 0, (int)stream.Length);
                            var ms5str = MakC.Common.mUtils.MD5Hash(bbf);
                            var filename = "/images/cards/" + ms5str + ext;
                            if (!System.IO.File.Exists(Environment.CurrentDirectory + "/wwwroot" + filename))
                            {
                                await System.IO.File.WriteAllBytesAsync(Environment.CurrentDirectory + "/wwwroot" + filename, bbf);
                            }
                            apiRes.data = filename;
                            apiRes.ok = true;
                        }
                    }
                }                               
            }
            catch (Exception ex)
            {
                apiRes.ok = false;
                apiRes.msg = ex.Message;
                apiRes.data = "";
            }
            return apiRes;
        }

        public ApiResult<ApiListObj<Ticket>> gettickets(int page, int limit)
        {
            ApiResult<ApiListObj<Ticket>> alc = new ApiResult<ApiListObj<Ticket>>();
            var dbh = DbContext.Get();
            alc.data = new ApiListObj<Ticket>();
            alc.data.totalCnt = dbh.Db.Queryable<Ticket>().Count();
            alc.data.items = dbh.Db.Queryable<Ticket>().ToList();
            alc.ok = true;
            return alc;
        }
        [HttpPost]
        public ApiResult<string> saveTicket(Ticket acard)
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
    public class ApiListObj<T>
    {
        public int totalCnt = 0;
        public List<T> items { get; set; }
    }
}