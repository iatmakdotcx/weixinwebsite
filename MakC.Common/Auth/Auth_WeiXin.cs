using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MakC.Common.Auth
{
    public class Auth_WeiXin
    {
        public static string appID = "";
        public static string appsecret = "";

        static Auth_WeiXin(){
            appID = ConfigHelper.Configuration["Weixin:appID"];
            appsecret = ConfigHelper.Configuration["Weixin:appsecret"];
        }

        public static string Get_openid(string code)
        {
            string url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={appID}&secret={appsecret}&code={code}&grant_type=authorization_code";
            string errMsg;
            string data = HttpGet(url,out errMsg);
            if (string.IsNullOrEmpty(errMsg))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(data);
                return jo["openid"]?.ToString();
            }
            return "";
        }

        public static string HttpGet(string url,out string errMsg)
        {
            errMsg = "";
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, err) => true;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "Get";                

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                errMsg = ex.Message;
                var log = new LogHelper();
                log.Error(typeof(Auth_WeiXin), errMsg);
                log.Error(typeof(Auth_WeiXin), ex.StackTrace);
            }
            return "";
        }
    }
}
