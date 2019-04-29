using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UW.Identity;
using UW.Shibboleth;

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

                return CreateClaimsPrincipal(attributes);
            } else
            {
                return (ClaimsPrincipal)context.User;
            }

        }

        /// <summary>
        /// Retrieves Shibboleth attributes from the Shibboleth session collection
        /// </summary>
        protected ShibbolethAttributeValueCollection GetAttributes(NameValueCollection collection)
        {
            var session_collection = collection.ToDictionary();

            return ShibbolethAttributeExtractor.ExtractAttributes(session_collection, ShibbolethDefaultAttributes.GetAttributeMapping());
        }

        /// <summary>
        /// Whether the session collection indicates this is a Shibboleth session
        /// </summary>
        /// <param name="collection"></param>
        protected abstract bool IsShibbolethSession(NameValueCollection collection);

        protected virtual ClaimsPrincipal CreateClaimsPrincipal(ShibbolethAttributeValueCollection collection)
        {
            ClaimsIdentity ident = new ClaimsIdentity("Shibboleth");

            if (collection.ContainsId("uid") && !collection.ValueIsNullOrEmpty("uid")) ident.AddClaim(new Claim(UWShibbolethClaimsType.UID, collection["uid"].Value.ToString().ToLower()));
            if (collection.ContainsId("givenName") && !collection.ValueIsNullOrEmpty("givenName")) ident.AddClaim(new Claim(UWShibbolethClaimsType.FIRSTNAME, collection["givenName"].Value));
            if (collection.ContainsId("sn") && !collection.ValueIsNullOrEmpty("sn")) ident.AddClaim(new Claim(UWShibbolethClaimsType.LASTNAME, collection["sn"].Value));
            if (collection.ContainsId("mail") && !collection.ValueIsNullOrEmpty("mail")) ident.AddClaim(new Claim(UWShibbolethClaimsType.EMAIL, collection["mail"].Value.ToString().ToLower()));
            if (collection.ContainsId("wiscEduPVI") && !collection.ValueIsNullOrEmpty("wiscEduPVI")) ident.AddClaim(new Claim(UWShibbolethClaimsType.PVI, collection["wiscEduPVI"].Value));
            if (collection.ContainsId("eppn") && !collection.ValueIsNullOrEmpty("eppn")) ident.AddClaim(new Claim(UWShibbolethClaimsType.EPPN, collection["eppn"].Value));

            if (collection.ContainsId("isMemberOf") && !collection.ValueIsNullOrEmpty("isMemberOf"))
            {
                string[] memberOf = collection["isMemberOf"].Value.ToString().Split(';');
                foreach (string member in memberOf)
                {
                    ident.AddClaim(new Claim(UWShibbolethClaimsType.Group, member));
                }
            }

            return new ClaimsPrincipal(ident);
        }
    }
}
