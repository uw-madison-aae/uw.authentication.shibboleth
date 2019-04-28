using System;
using System.Security.Claims;
using System.Web;

namespace UW.Authentication.AspNet
{
    /// <summary>
    /// Local development claims - must be overridden in each app to manually specify a user/claims
    /// </summary>
    public abstract class LocalDevClaimsAuthenticationHttpModule : ClaimsAuthenticationHttpModule
    {
        protected override ClaimsPrincipal GetClaimsPrincipal(HttpContext context)
        {
            return new ClaimsPrincipal(GetClaimsIdentity());
        }

        protected abstract ClaimsIdentity GetClaimsIdentity();
    }
}
