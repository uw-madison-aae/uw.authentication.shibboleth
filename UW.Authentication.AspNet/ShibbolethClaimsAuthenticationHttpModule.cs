using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UW.Identity;

namespace UW.Authentication.AspNet
{
    /// <summary>
    /// Base module for processing a request containing a Shbboleth user and populating a ClaimsPrincipal
    /// </summary>
    public abstract class ShibbolethClaimsAuthenticationHttpModule : ClaimsAuthenticationHttpModule
    {
        protected override ClaimsPrincipal GetClaimsPrincipal(HttpContext context)
        {
            

            // check if this is even a Shibboleth session
            if (IsShibbolethSession(context.Request.Headers))
            {
                var attributes = GetAttributes(context.Request.Headers);

                return processor.GetClaimsPrincipal();
            } else
            {
                return (ClaimsPrincipal)context.User;
            }

        }

        /// <summary>
        /// Retrieves Shibboleth attributes from the Shibboleth session collection
        /// </summary>
        protected IDictionary<string, string> GetAttributes(NameValueCollection collection)
        {
            return Shibboleth.ShibbolethAttributeExtractor.ExtractAttributes()
        }

        /// <summary>
        /// Whether the session collection indicates this is a Shibboleth session
        /// </summary>
        /// <param name="collection"></param>
        protected abstract bool IsShibbolethSession(NameValueCollection collection);
    }
}
