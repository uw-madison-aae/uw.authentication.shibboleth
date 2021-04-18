using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using UW.Shibboleth;
namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Processes Shibboleth authentication in Variable mode
    /// </summary>
    public class ShibbolethAuthenticationVariableProcessor : IShibbolethAuthenticationProcessor
    {
        protected HttpContext Context { get; }

        /// <summary>
        /// Shibboleth attribute ids and names from UW Shibboleth IdP
        /// </summary>
        protected IList<IShibbolethAttribute> Attributes { get; }

        public ShibbolethAuthenticationVariableProcessor(HttpContext httpContext, IList<IShibbolethAttribute> attributes)
        {
            Context = httpContext;
            Attributes = attributes;
        }

        public bool IsShibbolethSession()
        {
            // look for the presence of the Shib-Session-Index - indicates a Shibboleth session in effect
            return !string.IsNullOrEmpty(Context.GetServerVariable(ShibbolethAuthenticationDefaults.VariableShibIndexName));
        }

        /// <summary>
        /// Extracts Shibboleth attributes from a Shibboleth session context of server variables
        /// </summary>
        /// <returns>An <see cref="IDictionary{String,String}"/> for attributes and values</returns>
        public ShibbolethAttributeValueCollection GetAttributesFromRequest()
        {
            var ret_dict = new ShibbolethAttributeValueCollection();
            var distinct_ids = Attributes.GroupBy(a => a.Id).Select(a => a.First());
            foreach (var attrib in distinct_ids)
            {
                var value = Context.GetServerVariable(attrib.Id);
                if (!string.IsNullOrEmpty(value))
                {
                    ret_dict.Add(new ShibbolethAttributeValue(attrib.Id, value));
                }
            }

            return ret_dict;
        }
    }
}
