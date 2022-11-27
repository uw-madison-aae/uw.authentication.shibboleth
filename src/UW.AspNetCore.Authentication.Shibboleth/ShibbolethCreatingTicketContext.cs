using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class ShibbolethCreatingTicketContext : ResultContext<ShibbolethOptions>
    {
        public ShibbolethCreatingTicketContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ShibbolethOptions options,
            ClaimsPrincipal principal,
            AuthenticationProperties properties,
            ShibbolethAttributeValueCollection userData
            )
            : base(context,scheme, options)
        {
            Principal = principal;
            Properties = properties;
            UserData = userData;
        }

        /// <summary>
        /// Gets the collection of Shibboleth attributes found in the request
        /// </summary>
        public ShibbolethAttributeValueCollection UserData { get; }
    }
}
