using Microsoft.AspNetCore.Authentication;
using System;
using UW.Shibboleth;

namespace UW.Authentication.AspNetCore
{
    public static class ShibbolethExtensions
    {

        public static AuthenticationBuilder AddUWShibbolethForLinux(this AuthenticationBuilder builder)
            => builder.AddUWShibbolethForLinux(ShibbolethDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddUWShibbolethForLinux(this AuthenticationBuilder builder, Action<ShibbolethOptions> configureOptions)
            => builder.AddUWShibbolethForLinux(ShibbolethDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddUWShibbolethForLinux(this AuthenticationBuilder builder, string authenticationScheme, Action<ShibbolethOptions> configureOptions)
            => builder.AddUWShibbolethForLinux(authenticationScheme, displayName: ShibbolethDefaults.DisplayName, configureOptions: configureOptions);

        public static AuthenticationBuilder AddUWShibbolethForLinux(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ShibbolethOptions> configureOptions)
        {
            return builder.AddScheme<ShibbolethOptions, ShibbolethHeaderHandler>(authenticationScheme, displayName, configureOptions);
        }

        public static AuthenticationBuilder AddUWShibbolethForIISWithHeaders(this AuthenticationBuilder builder)
            => builder.AddUWShibbolethForIISWithHeaders(ShibbolethDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddUWShibbolethForIISWithHeaders(this AuthenticationBuilder builder, Action<ShibbolethOptions> configureOptions)
            => builder.AddUWShibbolethForIISWithHeaders(ShibbolethDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddUWShibbolethForIISWithHeaders(this AuthenticationBuilder builder, string authenticationScheme, Action<ShibbolethOptions> configureOptions)
            => builder.AddUWShibbolethForIISWithHeaders(authenticationScheme, displayName: ShibbolethDefaults.DisplayName, configureOptions: configureOptions);

        public static AuthenticationBuilder AddUWShibbolethForIISWithHeaders(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ShibbolethOptions> configureOptions)
        {
            return builder.AddScheme<ShibbolethOptions, ShibbolethHeaderHandler>(authenticationScheme, displayName, configureOptions);
        }

        public static AuthenticationBuilder AddUWShibbolethForIISWithVariables(this AuthenticationBuilder builder)
            => builder.AddUWShibbolethForIISWithVariables(ShibbolethDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddUWShibbolethForIISWithVariables(this AuthenticationBuilder builder, Action<ShibbolethOptions> configureOptions)
            => builder.AddUWShibbolethForIISWithVariables(ShibbolethDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddUWShibbolethForIISWithVariables(this AuthenticationBuilder builder, string authenticationScheme, Action<ShibbolethOptions> configureOptions)
            => builder.AddUWShibbolethForIISWithVariables(authenticationScheme, displayName: ShibbolethDefaults.DisplayName, configureOptions: configureOptions);

        public static AuthenticationBuilder AddUWShibbolethForIISWithVariables(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ShibbolethOptions> configureOptions)
        {
            return builder.AddScheme<ShibbolethOptions, ShibbolethVariableHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
