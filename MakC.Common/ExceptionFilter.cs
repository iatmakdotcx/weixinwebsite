using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MakC.Common
{
    public class ExceptionFilter
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerHelper log = new LogHelper();
       
        public ExceptionFilter(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {                
                log.Error(this, ex.Message);
                log.Error(this, ex.StackTrace);
                await HandleExceptionAsync(context, context.Response.StatusCode, ex.Message);
            }
            finally
            {
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string msg)
        {
            var data = new { statusCode, ok = false, msg = msg };
            context.Response.ContentType = "application/json;charset=utf-8";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(data));
        }
    }
}
