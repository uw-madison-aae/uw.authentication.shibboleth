using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;

namespace UW.Authentication.AspNetCore
{
    public class ShibbolethFailedContext : ResultContext<ShibbolethOptions>
    {
        public ShibbolethFailedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ShibbolethOptions options)
            : base(context, scheme, options) { }

        public Exception Exception { get; set; }
    }
}
