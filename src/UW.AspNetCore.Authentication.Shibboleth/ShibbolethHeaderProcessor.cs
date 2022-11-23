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
    public class ShibbolethHeaderProcessor : IShibbolethProcessor
    {
        protected HttpContext Context { get; }

        /// <summary>
        /// Shibboleth attribute ids from the IDP
        /// </summary>
        protected IShibbolethAttributeCollection Attributes { get; }

        public ShibbolethHeaderProcessor(HttpContext httpContext, IShibbolethAttributeCollection attributes)
        {
            Context = httpContext;
            Attributes = attributes;
        }

        public bool IsShibbolethSession()
        {
            // look for the presence of the ShibSessionIndex - indicates a Shibboleth session in effect
            if (Context.Request.Headers.TryGetValue(ShibbolethDefaults.HeaderShibIndexName, out StringValues shib_index))
            {
                return !StringValues.IsNullOrEmpty(shib_index);
            }

            return false;
        }

        public ShibbolethAttributeValueCollection GetAttributesFromRequest()
        {
            var headers = Context.Request.Headers;

            var attributeValues = new ShibbolethAttributeValueCollection();

            foreach(var attribute in Attributes)
            {
                if (headers.ContainsKey(attribute))
                {
                    attributeValues.Add(new ShibbolethAttributeValue(attribute, headers[attribute]));
                }
            }

            return attributeValues;
        }
    }
}
