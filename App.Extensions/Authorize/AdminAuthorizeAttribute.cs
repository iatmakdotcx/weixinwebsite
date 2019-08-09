using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Extensions
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public const string AuthenticationScheme = "AdminAuthenticationScheme";

        public AdminAuthorizeAttribute()
        {
            this.AuthenticationSchemes = AuthenticationScheme;
        }
    }
}
