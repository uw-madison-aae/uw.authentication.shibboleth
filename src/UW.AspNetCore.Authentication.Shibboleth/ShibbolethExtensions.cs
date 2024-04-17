using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace UW.AspNetCore.Authentication;

/// <summary>
/// Extension methods to add Shibboleth authentication capabilities to an HTTP application pipeline
/// </summary>
public static class ShibbolethExtensions
{
    /// <summary>
    /// Enables UW Shibboleth authentication using the default scheme <see cref="ShibbolethDefaults.AuthenticationScheme"/>
    /// </summary>
    /// <para>
    /// Shibboleth authentication performs authentication by extracting Shibboleth header values.
    /// </para>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddUWShibboleth(this AuthenticationBuilder builder)
    {
        return builder.AddUWShibboleth(ShibbolethDefaults.AuthenticationScheme, _ => { });
    }

    /// <summary>
    /// Enables UW Shibboleth authentication using a pre-defined scheme.
    /// <para>
    /// Shibboleth authentication performs authentication by extracting Shibboleth header values.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddUWShibboleth(this AuthenticationBuilder builder, string authenticationScheme)
        => builder.AddUWShibboleth(authenticationScheme, _ => { });

    /// <summary>
    /// Enables UW Shibboleth authentication using the default scheme <see cref="ShibbolethDefaults.AuthenticationScheme"/>
    /// <para>
    /// Shibboleth authentication performs authentication by extracting Shibboleth header values.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="configureOptions">A delegate that allows configuring <see cref="ShibbolethOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddUWShibboleth(this AuthenticationBuilder builder, Action<ShibbolethOptions> configureOptions)
        => builder.AddUWShibboleth(ShibbolethDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Enables UW Shibboleth authentication using the specified scheme.
    /// <para>
    /// Shibboleth authentication performs authentication by extracting Shibboleth header values.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="configureOptions">A delegate that allows configuring <see cref="ShibbolethOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddUWShibboleth(this AuthenticationBuilder builder, string authenticationScheme, Action<ShibbolethOptions> configureOptions)
        => builder.AddUWShibboleth(authenticationScheme, displayName: null, configureOptions: configureOptions);

    /// <summary>
    /// Enables UW Shibboleth authentication using the specified scheme.
    /// <para>
    /// Shibboleth authentication performs authentication by extracting Shibboleth header values.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="displayName">The display name for the authentication handler.</param>
    /// <param name="configureOptions">A delegate that allows configuring <see cref="ShibbolethOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddUWShibboleth(this AuthenticationBuilder builder,
        string authenticationScheme,
        string? displayName,
        Action<ShibbolethOptions> configureOptions)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<ShibbolethOptions>, ShibbolethPostConfigureOptions>());
        return builder.AddScheme<ShibbolethOptions, ShibbolethHandler>(authenticationScheme, displayName, configureOptions);
    }
}
