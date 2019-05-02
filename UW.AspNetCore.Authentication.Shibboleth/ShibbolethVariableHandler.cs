using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    public class ShibbolethVariableHandler : ShibbolethHandler
    {
        public ShibbolethVariableHandler(IOptionsMonitor<ShibbolethOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

        }

        public override bool IsShibbolethSession()
        {
            // look for the presence of the Shib-Session-Index - indicates a Shibboleth session in effect
            return !string.IsNullOrEmpty(Request.HttpContext.GetIISServerVariable("Shib-Session-ID"));
        }

        public override ShibbolethAttributeValueCollection GetAttributesFromRequest()
        {
            return ExtractAttributes(Request.HttpContext, GetShibbolethAttributes());
            
        }

        /// <summary>
        /// Extracts Shibboleth attributes from a Shibboleth session context of server variables
        /// </summary>
        /// <param name="attributes">A list of <see cref="IShibbolethAttribute"/> that is being extracted from the sessionCollection</param>
        /// <returns>An <see cref="IDictionary{String,String}"/> for attributes and values</returns>
        public ShibbolethAttributeValueCollection ExtractAttributes(HttpContext context, IEnumerable<IShibbolethAttribute> attributes)
        {
            var ret_dict = new ShibbolethAttributeValueCollection();
            var distinct_ids = attributes.GroupBy(a => a.Id).Select(a => a.First());
            foreach (var attrib in distinct_ids)
            {
                var value = context.GetIISServerVariable(attrib.Id);
                if (!string.IsNullOrEmpty(value))
                {
                    ret_dict.Add(new ShibbolethAttributeValue(attrib.Id, value));
                }
            }

            return ret_dict;
        }
    }
}
