using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using App.Extensions;
using MakC.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Website.Api
{
    [Route("SMSapi/[action]")]
   // [ApiController]
    public class SmsApiController : ControllerBase
    {
        private static string ApiPath = "https://dxyzm.market.alicloudapi.com/chuangxin/dxjk";
        private static string ApiMethod = "POST";
        private static string appcode = "fc91ba09d8a74f6eb6b1ff81821d02a2";
        private static string template = "【素亚LIFE】您的验证码是：{0}";

        private static int InvSec = 60;
        static SmsApiController()
        {
            ApiPath = Website.Models.SettingModel.getInstance().SMS_APIHost;
            appcode = Website.Models.SettingModel.getInstance().SMS_appcode;
            template = Website.Models.SettingModel.getInstance().SMS_template;
        }

        [HttpPost]
        public ApiResult<string> svcode(string tel, string vcode)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                if (string.IsNullOrEmpty(tel))
                {
                    apiRes.msg = $"手机号码无效(null)";
                    return apiRes;
                }
                tel = tel.Replace(" ", "").Replace("+86", "");
                if (tel.Length != 11 || tel[0] != '1')
                {
                    apiRes.msg = $"手机号码无效";
                    return apiRes;
                }

                string t = MemoryCacheService.Default.GetCache<string>("tel_" + tel);
                if (!string.IsNullOrEmpty(t))
                {
                    DateTime date = DateTime.Parse(t);
                    var ts = (DateTime.Now - date).TotalSeconds;
                    if (ts < InvSec)
                    {
                        apiRes.msg = $"{InvSec - (int)ts}秒后重试";
                        return apiRes;
                    }
                }
                MemoryCacheService.Default.SetCache("tel_" + tel,DateTime.Now.ToString(), 1);

                var r = new Random();
                int k = r.Next(100000, 999999);
                MemoryCacheService.Default.SetCache("vcode_" + tel, k.ToString(), 30);

                apiRes.msg = sendvcodeMsg(k.ToString(), tel);
                if (apiRes.msg != "")
                {
                    apiRes.ok = false;
                    return apiRes;
                }
                apiRes.ok = true;
                apiRes.data = InvSec.ToString();
            }
            catch (Exception ex)
            {
                apiRes.ok = false;
                apiRes.msg = ex.Message;
                apiRes.data = "";
            }
            return apiRes;
        }

        private static string HttpReqData(string url, string Method, string data, out string errMsg)
        {
            errMsg = "";
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, err) => true;
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = Method;
                request.Headers["Authorization"] = "APPCODE " + appcode;

                byte[] buffer = Encoding.UTF8.GetBytes(data);
                request.ContentLength = buffer.Length;
                using (var reqS = request.GetRequestStream())
                {
                    reqS.Write(buffer, 0, buffer.Length);
                }
                response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (response!= null && response.Headers["X-Ca-Error-Message"] != null)
                {
                    errMsg = response.Headers["X-Ca-Error-Message"];
                }
                else
                errMsg = ex.Message;
            }
            return "";
        }
        private static string sendvcodeMsg(string msg, string tel)
        {
            msg = System.Web.HttpUtility.UrlEncode(string.Format(template, msg));
            string querys = "?content=" + msg + "&mobile=" + tel;
            string errMsg;
            string bodys = HttpReqData(ApiPath + querys, ApiMethod, "", out errMsg);
            if (!string.IsNullOrEmpty(errMsg))
            {
                if (errMsg.IndexOf("mobile") > 0)
                {
                    return "手机号码不正确！";
                }
                return errMsg;
            }
            JObject resJson = (JObject)JsonConvert.DeserializeObject(bodys);
            if (resJson["ReturnStatus"].ToString() != "Success")
            {
                return resJson["Message"].ToString();
            }
            return "";
        }
    }
}