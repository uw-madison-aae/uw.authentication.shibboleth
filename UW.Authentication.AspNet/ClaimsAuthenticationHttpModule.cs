using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace UW.Authentication.AspNet
{
    /// <summary>
    /// Base class for Claims HTTP modules used for local identity management
    /// </summary>
    public abstract class ClaimsAuthenticationHttpModule : IHttpModule
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += Context_PostAuthenticateRequest;
        }

        private void Context_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;

            IPrincipal userPrincipal = GetClaimsPrincipal(context);

            // setting the Threading.Thread.CurrentPrincipal is unncessary as it gets set when the context.User is set
            context.User = userPrincipal;
        }

        /// <summary>
        /// Returns a <see cref="ClaimsPrincipal"/> to used by the SAM for authentication
        /// </summary>
        protected abstract IPrincipal GetClaimsPrincipal(HttpContext context);

    }
}
