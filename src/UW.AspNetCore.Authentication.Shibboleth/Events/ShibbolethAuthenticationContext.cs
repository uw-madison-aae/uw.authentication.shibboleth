using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace UW.AspNetCore.Authentication;

/// <summary>
/// Base context for Shibboleth Authentication
/// </summary>
public abstract class ShibbolethAuthenticationContext : HandleRequestContext<ShibbolethOptions>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="scheme">The authentication scheme.</param>
    /// <param name="options">The authentication options associated with the scheme.</param>
    /// <param name="properties">The authentication properties.</param>
    protected ShibbolethAuthenticationContext(
        HttpContext context,
        AuthenticationScheme scheme,
        ShibbolethOptions options,
        AuthenticationProperties? properties)
        : base(context, scheme, options)
        => Properties = properties ?? new AuthenticationProperties();

    /// <summary>
    /// Gets the <see cref="ClaimsPrincipal"/> containing the user claims.
    /// </summary>
    public ClaimsPrincipal? Principal { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="AuthenticationProperties"/>.
    /// </summary>
    public virtual AuthenticationProperties? Properties { get; set; }
}
