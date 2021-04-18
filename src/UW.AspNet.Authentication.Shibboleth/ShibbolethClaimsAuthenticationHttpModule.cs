using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using UW.Shibboleth;

namespace UW.AspNet.Authentication
{
    /// <summary>
    /// Base module for processing a request containing a Shbboleth user and populating a ClaimsPrincipal
    /// </summary>
    public abstract class ShibbolethClaimsAuthenticationHttpModule : ClaimsAuthenticationHttpModule
    {
        public override IPrincipal GetClaimsPrincipal(HttpContext context)
        {
            

            // check if this is even a Shibboleth session
            if (IsShibbolethSession(context.Request))
            {
                var attributes = GetAttributesFromRequest(context.Request);

                return CreateClaimsPrincipal(attributes);
            } else
            {
                return context.User;
            }

        }

        /// <summary>
        /// Retrieves Shibboleth attributes from the Shibboleth session collection
        /// </summary>
        public abstract ShibbolethAttributeValueCollection GetAttributesFromRequest(HttpRequest request);

        /// <summary>
        /// Whether the session collection indicates this is a Shibboleth session
        /// </summary>
        public abstract bool IsShibbolethSession(HttpRequest request);

        public virtual IList<IShibbolethAttribute> GetShibbolethAttributes()
        {
            return ShibbolethDefaultAttributes.GetAttributeMapping();

        }

        public virtual ClaimsPrincipal CreateClaimsPrincipal(ShibbolethAttributeValueCollection collection)
        {
            var ident = ShibbolethClaimsIdentityCreator.CreateIdentity(collection);
            return new ClaimsPrincipal(ident);
        }

        /// <summary>
        /// Extracts Shibboleth attributes from a Shibboleth session collection of headers/variables
        /// </summary>
        /// <param name="sessionCollection">A collection of headers/variables received in a Shibboleth session</param>
        /// <param name="attributes">A list of <see cref="IShibbolethAttribute"/> that is being extracted from the sessionCollection</param>
        /// <returns>An <see cref="IDictionary{String,String}"/> for attributes and values</returns>
        public ShibbolethAttributeValueCollection ExtractAttributes(NameValueCollection sessionCollection, IEnumerable<IShibbolethAttribute> attributes)
        {
            // quirk with ServerVariables for Shibboleth - cannot grabs the keys.  Keys are NOT added to the key collection.  Must request them manually (by supplied the ShibbolethAttribute.Id
            var ret_dict = new ShibbolethAttributeValueCollection();
            var distinct_ids = attributes.GroupBy(a => a.Id).Select(a => a.First());
            foreach (var attrib in distinct_ids)
            {
                if (sessionCollection[attrib.Id] != null)
                {
                    ret_dict.Add(new ShibbolethAttributeValue(attrib.Id, sessionCollection[attrib.Id]));
                }
            }

            return ret_dict;
        }
    }
}