using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace UW.AspNetCore.Authentication;

public class ShibbolethFailureContext : HandleRequestContext<ShibbolethOptions>
{
    public ShibbolethFailureContext(
        HttpContext context,
        AuthenticationScheme scheme,
        ShibbolethOptions options,
        Exception failure)
        : base(context, scheme, options)
    {
        Failure = failure;
    }

    /// <summary>
    /// User friendly error message for the error
    /// </summary>
    public Exception? Failure { get; set; }

    /// <summary>
    /// Additional state values for the authentication session.
    /// </summary>
    public AuthenticationProperties? Properties { get; set; }
}
