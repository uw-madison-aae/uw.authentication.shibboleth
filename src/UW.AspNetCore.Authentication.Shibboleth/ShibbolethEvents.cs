using Microsoft.AspNetCore.Authentication;
using System;
using System.Threading.Tasks;

namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Specifies events which the <see cref="ShibbolethHandler"/> invokes to enable developer control over the authentication process.
    /// </summary>
    public class ShibbolethEvents
    {
        /// <summary>
        /// Invoked if exceptions are thrown during request processing. The exceptions will be re-thrown after this event unless suppressed.
        /// </summary>
        public Func<ShibbolethFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the function that is invoked when the Authenticated method is invoked.
        /// </summary>
        public Func<ShibbolethCreatingTicketContext, Task> OnCreatingTicket { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the delegate that is invoked when the ApplyRedirect method is invoked.
        /// </summary>
        public Func<RedirectContext<ShibbolethOptions>, Task> OnRedirectToAuthorizationEndpoint { get; set; } = context =>
        {
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };

        /// <summary>
        /// Gets or sets the delegate that is invoked when attribute processor is being selected
        /// </summary>
        public Func<ShibbolethProcessorSelectionContext, Task> OnSelectingProcessor { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked whenever a user's Shibboleth information is found  in a session
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task CreatingTicket(ShibbolethCreatingTicketContext context) => OnCreatingTicket(context);

        /// <summary>
        /// Invoked when the attribute processor is being selected
        /// </summary>
        /// <param name="context">Contains information about the session</param>
        /// <returns></returns>
        public virtual Task ShibbolethProcessorSelection(ShibbolethProcessorSelectionContext context) => OnSelectingProcessor(context);

        /// <summary>
        /// Called when a Challenge causes a redirect to authorize endpoint in the Shibboleth handler
        /// </summary>
        /// <param name="context">Contains redirect URI and <see cref="AuthenticationProperties"/> of the challenge </param>
        public virtual Task RedirectToAuthorizationEndpoint(RedirectContext<ShibbolethOptions> context) => OnRedirectToAuthorizationEndpoint(context);

        public virtual Task AuthenticationFailed(ShibbolethFailedContext context) => OnAuthenticationFailed(context);
    }
}
