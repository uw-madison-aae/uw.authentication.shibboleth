using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Processes Shibboleth autehntication in Header mode
    /// </summary>
    public class ShibbolethHeaderProcessor : IShibbolethProcessor
    {

        /// <summary>
        /// Shibboleth attribute ids from the IDP
        /// </summary>
        protected IShibbolethAttributeCollection Attributes { get; }

        public ShibbolethHeaderProcessor(IShibbolethAttributeCollection attributes)
        {
            Attributes = attributes;
        }

        public bool IsShibbolethSession(HttpContext context)
        {
            // look for the presence of the ShibSessionIndex - indicates a Shibboleth session in effect
            if (context.Request.Headers.TryGetValue(ShibbolethDefaults.HeaderShibIndexName, out StringValues shib_index))
            {
                return !StringValues.IsNullOrEmpty(shib_index);
            }

            return false;
        }

        /// <summary>
        /// Extracts Shibboleth attributes from a Shibboleth session context of request headers
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ShibbolethAttributeValueCollection ExtractAttributeValues(HttpContext context)
        {
            var headers = context.Request.Headers;

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
