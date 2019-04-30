using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Encodings.Web;
using UW.Shibboleth;

namespace UW.Authentication.AspNetCore
{
    public class ShibbolethVariableHandler : ShibbolethHandler
    {
        public ShibbolethVariableHandler(IOptionsMonitor<ShibbolethOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

        }

        protected override bool IsShibbolethSession()
        {
            // look for the presence of the Shib-Session-Index - indicates a Shibboleth session in effect
            return !string.IsNullOrEmpty(Request.HttpContext.GetIISServerVariable("Shib-Session-ID"));

        }

        protected override ShibbolethAttributeValueCollection GetAttributesFromRequest()
        {
            //return ShibbolethAttributeExtractor.ExtractAttributes(request.Headers, GetShibbolethAttributes());
            throw new NotImplementedException();
        }
    }
}
