using Microsoft.AspNetCore.Authentication;

namespace UW.AspNetCore.Authentication;

/// <summary>
/// Default values for Shibboleth authentication middleware
/// </summary>
public static class ShibbolethDefaults
{
    /// <summary>
    /// Default value for <see cref="AuthenticationScheme.Name"/>.
    /// </summary>
    public const string AuthenticationScheme = "Shibboleth";

    /// <summary>
    /// Default value for <see cref="AuthenticationScheme.DisplayName"/>.
    /// </summary>
    public const string DisplayName = "Shibboleth";

    /// <summary>
    /// Default value for <see cref="AuthenticationSchemeOptions.ClaimsIssuer"/>.
    /// </summary>
    public const string Issuer = "Shibboleth";

    /// <summary>
    /// Value in the header indicating that a Shibboleth session is present
    /// </summary>
    /// <remarks>Used for Linux or (not recommended) IIS in Header mode</remarks>
    public const string HeaderShibIndexName = "ShibSessionIndex";

    /// <summary>
    /// Value in the IIS server variables indicating that a Shibboleth session is present
    /// </summary>
    /// <remarks>Used for IIS in variable mode - default starting in Shibboleth SP v3</remarks>
    public const string VariableShibIndexName = "Shib-Session-ID";
}
