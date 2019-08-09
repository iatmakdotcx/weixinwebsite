using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Website.Api
{
    [Route("wxapi/[action]")]
    [ApiController]

    public class wxApiController : ControllerBase
    {
        public string getwxid()
        {
            return "";
        }


    }
}