using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using UW.Shibboleth;

namespace UW.Authentication.AspNet
{
    /// <summary>
    /// Base module for processing a request containing a Shbboleth user and populating a ClaimsPrincipal
    /// </summary>
    public abstract class ShibbolethClaimsAuthenticationHttpModule : ClaimsAuthenticationHttpModule
    {
        protected override IPrincipal GetClaimsPrincipal(HttpContext context)
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
        protected abstract ShibbolethAttributeValueCollection GetAttributesFromRequest(HttpRequest request);

        /// <summary>
        /// Whether the session collection indicates this is a Shibboleth session
        /// </summary>
        protected abstract bool IsShibbolethSession(HttpRequest request);

        protected virtual IList<IShibbolethAttribute> GetShibbolethAttributes()
        {
            return ShibbolethDefaultAttributes.GetAttributeMapping();

        }

        protected virtual ClaimsPrincipal CreateClaimsPrincipal(ShibbolethAttributeValueCollection collection)
        {
            var ident = ShibbolethClaimsIdentityCreator.CreateIdentity(collection);
            return new ClaimsPrincipal(ident);
        }
    }
}