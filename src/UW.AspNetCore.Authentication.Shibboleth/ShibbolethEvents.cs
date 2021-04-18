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

        public virtual Task AuthenticationFailed(ShibbolethFailedContext context) => OnAuthenticationFailed(context);
    }
}
