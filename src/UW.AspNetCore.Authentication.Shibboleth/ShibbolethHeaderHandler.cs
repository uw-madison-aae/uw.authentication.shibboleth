using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
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
            return ExtractAttributes(Request.Headers, GetShibbolethAttributes());

        }

        /// <summary>
        /// Extracts Shibboleth attributes from a Shibboleth session collection of headers/variables
        /// </summary>
        /// <param name="sessionCollection">A collection of headers/variables received in a Shibboleth session</param>
        /// <param name="attributes">A list of <see cref="IShibbolethAttribute"/> that is being extracted from the sessionCollection</param>
        /// <returns>An <see cref="IDictionary{String,String}"/> for attributes and values</returns>
        public ShibbolethAttributeValueCollection ExtractAttributes(IDictionary<string, StringValues> sessionCollection, IEnumerable<IShibbolethAttribute> attributes)
        {
            var ret_dict = new ShibbolethAttributeValueCollection();
            var distinct_ids = attributes.GroupBy(a => a.Id).Select(a => a.First());
            foreach (var attrib in distinct_ids)
            {
                if (sessionCollection.ContainsKey(attrib.Id))
                {
                    ret_dict.Add(new ShibbolethAttributeValue(attrib.Id, sessionCollection[attrib.Id]));
                }
            }

            return ret_dict;
        }
    }
}
