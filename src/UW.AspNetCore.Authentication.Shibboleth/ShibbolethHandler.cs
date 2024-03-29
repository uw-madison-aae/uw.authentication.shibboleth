using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    public class ShibbolethHandler : AuthenticationHandler<ShibbolethOptions>
    {

#if NET8_0_OR_GREATER
        public ShibbolethHandler(IOptionsMonitor<ShibbolethOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {

        }
#else
        public ShibbolethHandler(IOptionsMonitor<ShibbolethOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

        }
#endif

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring. 
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new ShibbolethEvents Events
        {
            get => (ShibbolethEvents)base.Events!;
            set => base.Events = value;
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new ShibbolethEvents());

        /// <summary>
        /// Searches headers for Shibboleth parameters.  If found, an identity created with supplied information.
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {

                IShibbolethProcessor? shibbolethProcessor;
                var shibbolethAttributes = Options.ShibbolethAttributes;

                // Give the application opportunity to manually select the Shibboleth processor
                // typically used for development to manually create a Shibboleth session
                var processorSelectionContext = new ShibbolethProcessorSelectionContext(Context, Scheme, Options);
                await Events.ShibbolethProcessorSelection(processorSelectionContext);

                // if the application retrieved a Shibboleth processor from somewhere else, use that.
                shibbolethProcessor = processorSelectionContext.Processor;

                if (shibbolethProcessor == null || !shibbolethProcessor.IsShibbolethSession(Context)) 
                {
                    // check for variables first.  This is the preferred method when using IIS7 and considered more secure
                    // https://wiki.shibboleth.net/confluence/display/SP3/AttributeAccess#AttributeAccess-ServerVariables
                    shibbolethProcessor = new ShibbolethVariableProcessor(shibbolethAttributes);

                    // is Shibboleth enabled and in variable mode?
                    if (!shibbolethProcessor.IsShibbolethSession(Context))
                    {
                        // Shibboleth isn't enabled, or is in header mode.  Check header mode
                        shibbolethProcessor = new ShibbolethHeaderProcessor(shibbolethAttributes);

                        if (!shibbolethProcessor.IsShibbolethSession(Context))
                        {
                            // no Shibboleth sessions in header or variable mode.  Not authenticated.
                            // no result, as authentication may be handled by something else later
                            return AuthenticateResult.NoResult();
                        }
                    }

                }

                var identity = new ClaimsIdentity(ClaimsIssuer);
                var userData = shibbolethProcessor.ExtractAttributeValues(Context);

                return AuthenticateResult.Success(await CreateTicketAsync(identity, new AuthenticationProperties(), userData));

            } //end outer try
            catch (Exception ex)
            {
                Logger.ErrorProcessingMessage(ex);

                var authenticationFailedContext = new ShibbolethFailedContext(Context, Scheme, Options)
                {
                    Exception = ex
                };

                await Events.AuthenticationFailed(authenticationFailedContext);
                if (authenticationFailedContext.Result != null)
                {
                    return authenticationFailedContext.Result;
                }

                throw;
            } //end catch


        }

        protected virtual async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity, AuthenticationProperties properties, ShibbolethAttributeValueCollection userData)
        {
            // examine the specified user data, determine if requisite data is present, and optionally add it
            foreach (var action in Options.ClaimActions)
            {
                action.Run(userData, identity, Options.ClaimsIssuer ?? Scheme.Name);
            }

            var context = new ShibbolethCreatingTicketContext(Context, Scheme, Options, new ClaimsPrincipal(identity), properties, userData);
            await Events.CreatingTicket(context);

            return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);

        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (!Options.ProcessChallenge || !Options.ChallengePath.HasValue)
            {
                return base.HandleChallengeAsync(properties);
                
            }

            var redirectUri = OriginalPathBase + Options.ChallengePath;

            var redirectContext = new RedirectContext<ShibbolethOptions>(Context, Scheme, Options, properties, redirectUri);
            return Events.RedirectToAuthorizationEndpoint(redirectContext);

        }

    }
}
