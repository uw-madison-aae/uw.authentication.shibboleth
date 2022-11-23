using Microsoft.AspNetCore.Authentication;
using System;
using System.Diagnostics.CodeAnalysis;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Extension methods to add Shibboleth authentication capabilities to an HTTP application pipeline
    /// </summary>
    public static class ShibbolethExtensions
    {
        /// <summary>
        /// Adds <see cref="ShibbolethAuthenticationDefaults"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables UW Shibboleth authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddUWShibboleth([NotNull] this AuthenticationBuilder builder)
        {
            return builder.AddUWShibboleth(ShibbolethAuthenticationDefaults.AuthenticationScheme, options => { });
        }

        /// <summary>
        /// Adds <see cref="ShibbolethAuthenticationDefaults"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables UW Shibboleth authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="configuration">The delegate used to configure the Amazon options.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddUWShibboleth(
            [NotNull] this AuthenticationBuilder builder,
            [NotNull] Action<ShibbolethOptions> configuration)
        {
            return builder.AddUWShibboleth(ShibbolethAuthenticationDefaults.AuthenticationScheme, configuration);
        }

        /// <summary>
        /// Adds <see cref="ShibbolethHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables UW Shibboleth authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="scheme">The authentication scheme associated with this instance.</param>
        /// <param name="configuration">The delegate used to configure the Amazon options.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddUWShibboleth(
            [NotNull] this AuthenticationBuilder builder,
            [NotNull] string scheme,
            [NotNull] Action<ShibbolethOptions> configuration)
        {
            return builder.AddUWShibboleth(scheme, ShibbolethAuthenticationDefaults.DisplayName, configuration);
        }

        /// <summary>
        /// Add <see cref="ShibbolethHandler"/> to the specified <see cref="AuthenticationBuilder"/>, which enables UW Shibboleth authentication capabilities
        /// </summary>
        /// <param name="builder">The authentication builder</param>
        /// <param name="scheme">The authentication scheme associated with this instance.</param>
        /// <param name="caption">The optional display name associated with this instance.</param>
        /// <param name="configuration">The delegate used to configure the Amazon options.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddUWShibboleth(
            [NotNull] this AuthenticationBuilder builder,
            [NotNull] string scheme,
            string caption,
            [NotNull] Action<ShibbolethOptions> configuration)
        {
            return builder.AddScheme<ShibbolethOptions, ShibbolethHandler>(scheme, caption, configuration);
        }
    }
}
