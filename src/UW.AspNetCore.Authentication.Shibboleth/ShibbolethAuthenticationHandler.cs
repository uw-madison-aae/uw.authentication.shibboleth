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
    public class ShibbolethAuthenticationHandler : AuthenticationHandler<ShibbolethAuthenticationOptions>
    {
        public ShibbolethAuthenticationHandler(IOptionsMonitor<ShibbolethAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
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

        /// <inheritdoc />
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {

                IShibbolethAuthenticationProcessor shibbolethProcessor;
                var shibbolethAttributes = GetShibbolethAttributes();

                // check for variables first.  This is the preferred method when using IIS7 and considered more secure
                // https://wiki.shibboleth.net/confluence/display/SP3/AttributeAccess#AttributeAccess-ServerVariables
                shibbolethProcessor = new ShibbolethAuthenticationVariableProcessor(Context, shibbolethAttributes);

                // is Shibboleth enabled and in variable mode?
                if (!shibbolethProcessor.IsShibbolethSession())
                {
                    // Shibboleth isn't enabled, or is in header mode.  Check header mode
                    shibbolethProcessor = new ShibbolethAuthenticationHeaderProcessor(Context, shibbolethAttributes);

                    if (!shibbolethProcessor.IsShibbolethSession())
                    {
                        // no Shibboleth sessions in header or variable mode.  Not authenticated.
                        // no result, as authentication may be handled by something else later
                        return Task.FromResult(AuthenticateResult.NoResult());
                    }
                }

                return Task.FromResult(
                     AuthenticateResult.Success(
                        new AuthenticationTicket(
                            CreateClaimsPrincipal(shibbolethProcessor.GetAttributesFromRequest()),
                            new AuthenticationProperties(),
                            Scheme.Name)));

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

        /// <summary>
        /// Creates a <see cref="ClaimsIdentity"/> using the minimum Shibboleth attributes
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public virtual ClaimsPrincipal CreateClaimsPrincipal(ShibbolethAttributeValueCollection userData)
        {
            var identity = new ClaimsIdentity(Scheme.Name);

            // examine the specified user data, determine if requisite data is present, and optionally add it
            foreach(var action in Options.ClaimActions)
            {
                action.Run(userData, identity, Options.ClaimsIssuer ?? Scheme.Name);
            }

            return new ClaimsPrincipal(identity);
        }

        /// <summary>
        /// Returns the attribute mapping from the XML stored in UW.Shibboleth
        /// </summary>
        public IList<IShibbolethAttribute> GetShibbolethAttributes()
        {
            return ShibbolethDefaultAttributes.GetAttributeMapping();

        }

    }
}
