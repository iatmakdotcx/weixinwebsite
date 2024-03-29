﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Extensions;
using MakC.Common;
using App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;

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
                //ConfigHelper.Configuration["Weixin:appID"] = appid;
                //ConfigHelper.Configuration["Weixin:appsecret"] = key;
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
        public ApiResult<string> sms(string url, string key, string template)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                int optc = dbh.Db.Updateable<Setting>().SetColumns(ii => ii.value == url).Where(ii => ii.key == "SMS_APIHost").ExecuteCommand();
                if (optc == 0)
                {
                    dbh.Db.Insertable(new Setting() { key = "SMS_APIHost", value = url }).ExecuteCommand();
                }
                optc = dbh.Db.Updateable<Setting>().SetColumns(ii => ii.value == key).Where(ii => ii.key == "SMS_appcode").ExecuteCommand();
                if (optc == 0)
                {
                    dbh.Db.Insertable(new Setting() { key = "SMS_appcode", value = key }).ExecuteCommand();
                }
                optc = dbh.Db.Updateable<Setting>().SetColumns(ii => ii.value == template).Where(ii => ii.key == "SMS_template").ExecuteCommand();
                if (optc == 0)
                {
                    dbh.Db.Insertable(new Setting() { key = "SMS_template", value = template }).ExecuteCommand();
                }
                Website.Models.SettingModel.clearCache();
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
                Website.Models.SettingModel.clearCache();
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
        public ApiResult<string> firstpage(string data)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                int optc = dbh.Db.Updateable<Setting>().SetColumns(ii => ii.value == data).Where(ii => ii.key == "mobile_FirstPage").ExecuteCommand();
                if (optc == 0)
                {
                    dbh.Db.Insertable(new Setting() { key = "mobile_FirstPage", value = data }).ExecuteCommand();
                }
                Website.Models.SettingModel.clearCache();
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
        public ApiResult<string> tel(string yoga_tel,string skc_tel)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var dbh = DbContext.Get();
                int optc = dbh.Db.Updateable<Setting>().SetColumns(ii => ii.value == yoga_tel).Where(ii => ii.key == "Yoga_Tel").ExecuteCommand();
                if (optc == 0)
                {
                    dbh.Db.Insertable(new Setting() { key = "Yoga_Tel", value = yoga_tel }).ExecuteCommand();
                }
                optc = dbh.Db.Updateable<Setting>().SetColumns(ii => ii.value == skc_tel).Where(ii => ii.key == "skincare_Tel").ExecuteCommand();
                if (optc == 0)
                {
                    dbh.Db.Insertable(new Setting() { key = "skincare_Tel", value = skc_tel }).ExecuteCommand();
                }
                Website.Models.SettingModel.clearCache();
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
            alc.data.items = dbh.Db.Queryable<Card>().ToPageList(page, limit, ref alc.data.totalCnt);
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
        public async Task<ApiResult<string>> uploadimg(int p=0)
        {
            string imgdir;
            switch (p)
            {
                case 1: imgdir = "cards"; break;
                case 2: imgdir = "class"; break;
                case 3: imgdir = "classinfo"; break;
                case 4: imgdir = "userAcatar"; break;
                default:
                    imgdir = "cards";
                    break;
            }
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
                            var filename = "/images/" + imgdir + "/" + ms5str + ext;
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
        [HttpPost]
        public ContentResult UMuploadimg(string editorid = "", string callback = "")
        {
            var upq = uploadimg(3).Result;

            if (upq.ok)
            {
                var jsonData= new JObject(
                    new JProperty("state", "SUCCESS"),
                    new JProperty("url", upq.data.Substring(1)),
                    new JProperty("originalName", "originalName"),
                    new JProperty("name", Path.GetFileName(upq.data)),
                    new JProperty("size", "0"),
                    new JProperty("type", Path.GetExtension(upq.data))
                    );
                HttpContext.Response.ContentType = "text/html";
                if (string.IsNullOrEmpty(callback))
                {
                    return Content(jsonData.ToString(Newtonsoft.Json.Formatting.None));
                }
                else
                {
                    return Content($"<script>{callback}(JSON.parse(\"{jsonData.ToString(Newtonsoft.Json.Formatting.None).Replace("\"","\\\"")}\"));</script>");
                }
            }
            return Content("");
        }

        public ApiResult<ApiListObj<Ticket>> gettickets(int page, int limit)
        {
            ApiResult<ApiListObj<Ticket>> alc = new ApiResult<ApiListObj<Ticket>>();
            var dbh = DbContext.Get();
            alc.data = new ApiListObj<Ticket>();            
            alc.data.items = dbh.Db.Queryable<Ticket>().ToPageList(page, limit, ref alc.data.totalCnt);
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
 
        [HttpPost]
        public ApiResult<string> distributecard(string cardids, string userids)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var insertSql = @"INSERT INTO usercards(userid,create_at,cardsid,cardname,cardtype,cardexpiryDate,canUseCnt,usedCnt)
select users.id,NOW(),cards.id,cards.`name`,cards.type,DATE_ADD(NOW(),INTERVAL cards.expiry DAY),cards.canUseCount,0
from users JOIN cards where users.id in ("+ mUtils.checkids(userids) + ") and cards.id in (" + mUtils.checkids(cardids) + ")";

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
        public ApiResult<string> distributeticket(string cardids, string userids)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                var insertSql = @"INSERT INTO usertickets(userid,ticketid,create_at,expiryDate,deleted)
select users.id,tickets.id,NOW(),DATE_ADD(NOW(),INTERVAL tickets.expiryDay DAY),0
from users JOIN tickets where users.id in (" + mUtils.checkids(userids) + ") and tickets.id in (" + mUtils.checkids(cardids) + ")";

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
    }
    public class ApiListObj<T>
    {
        public int totalCnt = 0;
        public List<T> items { get; set; }
    }
}