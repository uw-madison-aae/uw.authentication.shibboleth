using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    public abstract class ShibbolethHandler : AuthenticationHandler<ShibbolethOptions>
    {

        public ShibbolethHandler(IOptionsMonitor<ShibbolethOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

        }

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring. 
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new ShibbolethEvents Events
        {
            get { return (ShibbolethEvents)base.Events; }
            set { base.Events = value; }
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new ShibbolethEvents());

        /// <summary>
        /// Searches for the 'ShibSessionIndex' header. If the ShibSessionIndex header is found, remoteuser is used to make an authentication ticket.
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                // check if this request has Shibboleth enabled
                if (!IsShibbolethSession())
                {
                    // no result, as authentication may be handled by something else later
                    return Task.FromResult(AuthenticateResult.NoResult());
                }


                return Task.FromResult(
                     AuthenticateResult.Success(
                        new AuthenticationTicket(
                            //new ClaimsPrincipal(Options.Identity),
                            //new ClaimsPrincipal(),
                            GetClaimsPrincipal(),
                            new AuthenticationProperties(),
                            this.Scheme.Name)));

            } //end outer try
            catch (Exception ex)
            {
                Logger.ErrorProcessingMessage(ex);

                var authenticationFailedContext = new ShibbolethFailedContext(Context, Scheme, Options)
                {
                    Exception = ex
                };

                Events.AuthenticationFailed(authenticationFailedContext);
                if (authenticationFailedContext.Result != null)
                {
                    return Task.FromResult(authenticationFailedContext.Result);
                }

                throw;
            } //end catch


        }

        public virtual ClaimsPrincipal GetClaimsPrincipal()
        {
            var attributes = GetAttributesFromRequest();
            
            return CreateClaimsPrincipal(attributes);
        }

        public abstract bool IsShibbolethSession();

        public abstract ShibbolethAttributeValueCollection GetAttributesFromRequest();

        public virtual IList<IShibbolethAttribute> GetShibbolethAttributes()
        {
            return ShibbolethDefaultAttributes.GetAttributeMapping();

        }

        public virtual ClaimsPrincipal CreateClaimsPrincipal(ShibbolethAttributeValueCollection collection)
        {
            var ident = ShibbolethClaimsIdentityCreator.CreateIdentity(collection);
            return new ClaimsPrincipal(ident);
        }

    }
}
