using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Extensions;
using MakC.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Website.Api
{
    [Route("SMSapi/[action]")]
   // [ApiController]
    public class SmsApiController : ControllerBase
    {
        private static int InvSec = 60;

        [HttpPost]
        public ApiResult<string> svcode(string tel, string vcode)
        {
            var apiRes = new ApiResult<string>();
            try
            {
                if (string.IsNullOrEmpty(tel))
                {
                    apiRes.msg = $"手机号码无效";
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

                //TODO:发送短信
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
    }
}