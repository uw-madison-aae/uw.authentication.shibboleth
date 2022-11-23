using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using UW.Shibboleth;
namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Processes Shibboleth authentication in Variable mode
    /// </summary>
    public class ShibbolethVariableProcessor : IShibbolethProcessor
    {
        protected HttpContext Context { get; }

        /// <summary>
        /// Shibboleth attribute ids from the IDP
        /// </summary>
        protected IShibbolethAttributeCollection Attributes { get; }

        public ShibbolethVariableProcessor(HttpContext httpContext, IShibbolethAttributeCollection attributes)
        {
            Context = httpContext;
            Attributes = attributes;
        }

        public bool IsShibbolethSession()
        {
            // look for the presence of the Shib-Session-Index - indicates a Shibboleth session in effect
            return !string.IsNullOrEmpty(Context.GetServerVariable(ShibbolethDefaults.VariableShibIndexName));
        }

        /// <summary>
        /// Extracts Shibboleth attributes from a Shibboleth session context of server variables
        /// </summary>
        /// <returns>An <see cref="IDictionary{String,String}"/> for attributes and values</returns>
        public ShibbolethAttributeValueCollection GetAttributesFromRequest()
        {
            var attributeValues = new ShibbolethAttributeValueCollection();

            foreach (var attribute in Attributes)
            {
                var value = Context.GetServerVariable(attribute);
                if (!string.IsNullOrEmpty(value))
                {
                    attributeValues.Add(new ShibbolethAttributeValue(attribute, value));
                }
            }

            return attributeValues;
        }
    }
}
