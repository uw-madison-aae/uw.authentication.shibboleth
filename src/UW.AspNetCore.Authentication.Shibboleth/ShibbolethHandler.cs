using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication;

public class ShibbolethHandler : AuthenticationHandler<ShibbolethOptions>,
    IAuthenticationRequestHandler,
    IAuthenticationSignInHandler,
    IAuthenticationSignOutHandler
{
    private Task<HandleRequestResult>? _extractShibbolethDataTask;
    private const string AuthSchemeKey = ".AuthScheme";

    /// <summary>
    /// The authentication scheme used by default for signin.
    /// </summary>
    protected string? SignInScheme => Options.SignInScheme;

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
    /// Gets a value that determines if the current authentication request should be handled by <see cref="HandleRequestAsync" />.
    /// </summary>
    /// <returns><see langword="true"/> to handle the operation, otherwise <see langword="false"/>.</returns>
    public virtual Task<bool> ShouldHandleRequestAsync()
        => Task.FromResult(!Options.UseChallenge
            || (Options.UseChallenge && Options.CallbackPath == Request.Path));

    /// <summary>
    /// Handles the current authentication request.
    /// </summary>
    /// <returns><see langword="true"/> if authentication was handled, otherwise <see langword="false"/>.</returns>
    public virtual async Task<bool> HandleRequestAsync()
    {
        if (!await ShouldHandleRequestAsync())
            return false;

        AuthenticationTicket? ticket = null;
        Exception? exception = null;
        AuthenticationProperties? properties = null;
        try
        {
            // taken from RemoteAuthenticationHandler
            // https://github.com/dotnet/aspnetcore/blob/3f16e780a3a708d1d63fb4458157178c7fab5f39/src/Security/Authentication/Core/src/RemoteAuthenticationHandler.cs#L87
            HandleRequestResult authResult = await EnsureShibbolethSession();
            if (authResult == null)
            {
                exception = new InvalidOperationException("Invalid return state, unable to redirect.");
            }
            else if (authResult.Handled)
            {
                return true;
            }
            else if (authResult.Skipped || authResult.None)
            {
                return false;
            }
            else if (!authResult.Succeeded)
            {
                exception = authResult.Failure ?? new InvalidOperationException("Invalid return state, unable to redirect.");
                properties = authResult.Properties;
            }

            ticket = authResult?.Ticket;
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        if (exception != null)
        {
            Logger.ShibbolethAuthenticationFailed(exception.Message);
            var errorContext = new ShibbolethFailureContext(Context, Scheme, Options, exception)
            {
                Properties = properties
            };
            await Events.ShibbolethFailure(errorContext);

            if (errorContext.Result != null)
            {
                if (errorContext.Result.Handled)
                {
                    return true;
                }
                else if (errorContext.Result.Skipped)
                {
                    return false;
                }
                else if (errorContext.Result.Failure != null)
                {
#if NET8_0_OR_GREATER
                    throw new AuthenticationFailureException("An error was returned from the ShibbolethFailure event.", errorContext.Result.Failure);
#else
                    throw new Exception("An error was returned from the ShibbolethFailure event.", errorContext.Result.Failure);
#endif
                }
            }

            if (errorContext.Failure != null)
            {
#if NET8_0_OR_GREATER
                throw new AuthenticationFailureException("An error was encountered while handling the Shibboleth login.", errorContext.Failure);
#else
                throw new Exception("An error was encountered while handling the Shibboleth login.", errorContext.Failure);
#endif
            }
        }

        // We have a ticket if we get here
        Debug.Assert(ticket != null);
        var ticketContext = new ShibbolethTicketReceivedContext(Context, Scheme, Options, ticket)
        {
            ReturnUri = Request.Query[Options.ReturnUrlParameter]
        };

        ticket.Properties.RedirectUri = null;

        // Mark which provider produced this identity so we can cross-check later in HandleAuthenticateAsync
        ticketContext.Properties!.Items[AuthSchemeKey] = Scheme.Name;

        await Events.TicketReceived(ticketContext);

        if (ticketContext.Result != null)
        {
            if (ticketContext.Result.Handled)
            {
                Logger.SignInHandled();
                return true;
            }
            else if (ticketContext.Result.Skipped)
            {
                Logger.SignInSkipped();
                return false;
            }
        }

        await Context.SignInAsync(SignInScheme, ticketContext.Principal!, ticketContext.Properties);

        // Default redirect path is the base path
        if (string.IsNullOrEmpty(ticketContext.ReturnUri))
        {
            ticketContext.ReturnUri = BuildRedirectUri("/");
        }

        // if this the entire site is protected, we have already extracted the Shibboleth information
        // however, we are saying the request "wasn't handled" so that AuthenticateAsync() will also run
        if (!Options.UseChallenge)
            return false;

        Response.Redirect(ticketContext.ReturnUri);
        return true;
    }

    /// <summary>
    /// Ensures that a Shibboleth session is in place
    /// </summary>
    /// <returns></returns>
    /// <remarks>Modeled after CookieAuthenticationHandler
    /// https://github.com/dotnet/aspnetcore/blob/3f16e780a3a708d1d63fb4458157178c7fab5f39/src/Security/Authentication/Cookies/src/CookieAuthenticationHandler.cs#L82</remarks>
    private Task<HandleRequestResult> EnsureShibbolethSession()
    {
        // We only need to extract the Shibboleth values once
        // only useful in an entire-site configuration
        _extractShibbolethDataTask ??= ExamineForShibbolethSession();
        return _extractShibbolethDataTask;
    }

    private async Task<HandleRequestResult> ExamineForShibbolethSession()
    {
        IShibbolethProcessor? shibbolethProcessor;
        IShibbolethAttributeCollection shibbolethAttributes = Options.ShibbolethAttributes;

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
                    return HandleRequestResult.NoResult();
                }
            }
        }

        var identity = new ClaimsIdentity(ClaimsIssuer);
        ShibbolethAttributeValueCollection userData = shibbolethProcessor.ExtractAttributeValues(Context);

        return HandleRequestResult.Success(await CreateTicketAsync(identity, new AuthenticationProperties(), userData));
    }

    /// <summary>
    /// Searches headers for Shibboleth parameters.  If found, an identity created with supplied information.
    /// </summary>
    /// <returns></returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            return AuthenticateResult.NoResult();
        }

        // if using challenge, another handler will be handling authentication (typically cookie middleware)
        if (Options.UseChallenge)
        {
            // taken from RemoteAuthenticationHandler
            // https://github.com/dotnet/aspnetcore/blob/f98b1df4aca8b0919173a4a96f0b094a0c011308/src/Security/Authentication/Core/src/RemoteAuthenticationHandler.cs#L195
            AuthenticateResult result = await Context.AuthenticateAsync(SignInScheme);
            if (result != null)
            {
                if (result.Failure != null)
                {
                    return result;
                }

                // The SignInScheme may be shared with multiple providers, make sure this provider issued the identity.
                AuthenticationTicket? ticket = result.Ticket;
                if (ticket != null && ticket.Principal != null && ticket.Properties != null
                    && ticket.Properties.Items.TryGetValue(AuthSchemeKey, out string? authenticatedScheme)
                    && string.Equals(Scheme.Name, authenticatedScheme, StringComparison.Ordinal))
                {
                    return AuthenticateResult.Success(new AuthenticationTicket(ticket.Principal,
                        ticket.Properties, Scheme.Name));
                }

                return AuthenticateResult.NoResult();
            }
        }

        // Now assume it's an entire-site Shibboleth
        // Check to see if the Shibboleth session is valid
        try
        {
            HandleRequestResult result = await EnsureShibbolethSession();
            return result;
        } //end outer try
        catch (Exception ex)
        {
            var authenticationFailedContext = new ShibbolethFailureContext(Context, Scheme, Options, ex)
            {
                Failure = ex
            };

            await Events.ShibbolethFailure(authenticationFailedContext);
            if (authenticationFailedContext.Result != null)
            {
                return authenticationFailedContext.Result;
            }

            throw;
        } //end catch
    }

    /// <summary>
    /// Creates the <see cref="AuthenticationTicket"/> with the supplied identity and attributes
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="properties"></param>
    /// <param name="userData"></param>
    /// <returns></returns>
    protected virtual async Task<AuthenticationTicket> CreateTicketAsync(
        ClaimsIdentity identity, AuthenticationProperties properties, ShibbolethAttributeValueCollection userData)
    {
        // examine the specified user data, determine if requisite data is present, and optionally add it
        foreach (ShibbolethClaimAction action in Options.ClaimActions)
        {
            action.Run(userData, identity, Options.ClaimsIssuer ?? Scheme.Name);
        }

        var context = new ShibbolethCreatingTicketContext(Context, Scheme, Options, new ClaimsPrincipal(identity), properties, userData);
        await Events.CreatingTicket(context);

        return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        if (!Options.UseChallenge || !Options.CallbackPath.HasValue)
        {
            return base.HandleChallengeAsync(properties);
        }

        if (string.IsNullOrEmpty(properties.RedirectUri))
        {
            properties.RedirectUri = OriginalPath + Request.QueryString;
        }

        string authorizationEndpoint = BuildChallengeUrl(properties, properties.RedirectUri);

        var redirectContext = new RedirectContext<ShibbolethOptions>(
            Context, Scheme, Options,
            properties, authorizationEndpoint);
        return Events.RedirectToAuthorizationEndpoint(redirectContext);
    }

    /// <summary>
    /// Constructs the simulated OAuth challenge url.  This should be a Shibboleth-protected endpoint
    /// </summary>
    /// <param name="properties">The <see cref="AuthenticationProperties"/>.</param>
    /// <param name="redirectUri">The url to redirect to once the challenge is completed.</param>
    /// <returns>The challenge url</returns>
    /// <remarks>Modeled off of OAuthHandler.cs
    /// https://github.com/dotnet/aspnetcore/blob/6196f76672ed4a4415f7a12e8ae17b8212ebf462/src/Security/Authentication/OAuth/src/OAuthHandler.cs#L300
    /// </remarks>
    protected string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
    {
        var parameters = new Dictionary<string, string>()
        {
            { Options.ReturnUrlParameter, redirectUri }
        };

        PathString authorizationEndpoint = OriginalPathBase + Options.CallbackPath;

        return QueryHelpers.AddQueryString(authorizationEndpoint, parameters!);
    }

    /// <inheritdoc/>
    public virtual Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties? properties)
    {
        // forward the sign-in if this is a challenge
        if (Options.UseChallenge)
            return Context.SignInAsync(Options.SignInScheme, user, properties);

        // Not a challenge, but an entire site protected.  Only need to "fake" sign-in
        return Task.CompletedTask;
    }

    public Task SignOutAsync(AuthenticationProperties? properties)
    {
        // forward the sign-out if this is a challenge
        if (Options.UseChallenge)
            return Context.SignOutAsync(Options.SignInScheme, properties);

        // Not a challenge, but an entire site protected.  Only need to "fake" a sign-out
        return Task.CompletedTask;
    }
}
