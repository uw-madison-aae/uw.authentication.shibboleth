using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace UW.AspNetCore.Authentication;

public class ShibbolethTicketReceivedContext : ShibbolethAuthenticationContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="TicketReceivedContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <param name="scheme">The <see cref="AuthenticationScheme"/>.</param>
    /// <param name="options">The <see cref="ShibbolethOptions"/>.</param>
    /// <param name="ticket">The received ticket.</param>
    public ShibbolethTicketReceivedContext(
        HttpContext context,
        AuthenticationScheme scheme,
        ShibbolethOptions options,
        AuthenticationTicket ticket)
        : base(context, scheme, options, ticket?.Properties)
        => Principal = ticket?.Principal;

    /// <summary>
    /// Gets or sets the URL to redirect to after signin.
    /// </summary>
    public string? ReturnUri { get; set; }
}
