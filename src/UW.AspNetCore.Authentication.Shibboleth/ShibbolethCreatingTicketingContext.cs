using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class ShibbolethCreatingTicketingContext : ResultContext<ShibbolethAuthenticationOptions>
    {
        public ShibbolethCreatingTicketingContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ShibbolethAuthenticationOptions options,
            ClaimsPrincipal principal,
            AuthenticationProperties properties
            )
            : base(context,scheme, options)
        {
            Principal = principal;
            Properties = properties;
        }
    }
}
