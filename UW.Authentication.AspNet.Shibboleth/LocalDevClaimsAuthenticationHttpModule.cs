using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace UW.Authentication.AspNet
{
    /// <summary>
    /// Local development claims - must be overridden in each app to manually specify a user/claims
    /// </summary>
    public abstract class LocalDevClaimsAuthenticationHttpModule : ClaimsAuthenticationHttpModule
    {
        public override IPrincipal GetClaimsPrincipal(HttpContext context)
        {
            return new ClaimsPrincipal(GetClaimsIdentity());
        }

        public abstract ClaimsIdentity GetClaimsIdentity();
    }
}
