using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;

namespace UW.AspNetCore.Authentication
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
