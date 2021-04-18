using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Processes Shibboleth autehntication in Header mode
    /// </summary>
    public class ShibbolethAuthenticationHeaderProcessor : IShibbolethAuthenticationProcessor
    {
        protected HttpContext Context { get; }

        /// <summary>
        /// Shibboleth attribute ids and names from UW Shibboleth IdP
        /// </summary>
        protected IList<IShibbolethAttribute> Attributes { get; }

        public ShibbolethAuthenticationHeaderProcessor(HttpContext httpContext, IList<IShibbolethAttribute> attributes)
        {
            Context = httpContext;
            Attributes = attributes;
        }

        public bool IsShibbolethSession()
        {
            // look for the presence of the ShibSessionIndex - indicates a Shibboleth session in effect
            if (Context.Request.Headers.TryGetValue(ShibbolethAuthenticationDefaults.HeaderShibIndexName, out StringValues shib_index))
            {
                return !StringValues.IsNullOrEmpty(shib_index);
            }

            return false;
        }

        public ShibbolethAttributeValueCollection GetAttributesFromRequest()
        {
            var headers = Context.Request.Headers;

            var ret_dict = new ShibbolethAttributeValueCollection();
            var distinct_ids = Attributes.GroupBy(a => a.Id).Select(a => a.First());
            foreach (var attrib in distinct_ids)
            {
                if (headers.ContainsKey(attrib.Id))
                {
                    ret_dict.Add(new ShibbolethAttributeValue(attrib.Id, headers[attrib.Id]));
                }
            }

            return ret_dict;
        }
    }
}
