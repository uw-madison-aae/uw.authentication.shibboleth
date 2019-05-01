using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using UW.Shibboleth;

namespace UW.Authentication.AspNetCore
{
    public class ShibbolethHeaderHandler : ShibbolethHandler
    {
        public ShibbolethHeaderHandler(IOptionsMonitor<ShibbolethOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

        }

        public override bool IsShibbolethSession()
        {
            // look for the presence of the ShibSessionIndex - indicates a Shibboleth session in effect
            if (Request.Headers.TryGetValue("ShibSessionIndex", out StringValues shib_index))
            {
                return !StringValues.IsNullOrEmpty(shib_index);
            }

            return false;
        }
        public override ShibbolethAttributeValueCollection GetAttributesFromRequest()
        {
            //return ShibbolethAttributeExtractor.ExtractAttributes(request.Headers, GetShibbolethAttributes());
            throw new NotImplementedException();
        }
    }
}
