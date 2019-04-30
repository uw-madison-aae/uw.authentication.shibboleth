using Microsoft.AspNetCore.Authentication;
using System;

namespace UW.Authentication.AspNetCore
{
    public static class ShibbolethExtensions
    {

        public static AuthenticationBuilder AddUWShibbolethWithVariables(this AuthenticationBuilder builder)
            => builder.AddUWShibbolethWithVariables(ShibbolethDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddUWShibbolethWithVariables(this AuthenticationBuilder builder, Action<ShibbolethOptions> configureOptions)
            => builder.AddUWShibbolethWithVariables(ShibbolethDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddUWShibbolethWithVariables(this AuthenticationBuilder builder, string authenticationScheme, Action<ShibbolethOptions> configureOptions)
            => builder.AddUWShibbolethWithVariables(authenticationScheme, displayName: ShibbolethDefaults.DisplayName, configureOptions: configureOptions);

        public static AuthenticationBuilder AddUWShibbolethWithVariables(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ShibbolethOptions> configureOptions)
        {
            return builder.AddScheme<ShibbolethOptions, ShibbolethVariableHandler>(authenticationScheme, displayName, configureOptions);
        }

        public static AuthenticationBuilder AddUWShibbolethWithHeaders(this AuthenticationBuilder builder)
            => builder.AddUWShibbolethWithHeaders(ShibbolethDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddUWShibbolethWithHeaders(this AuthenticationBuilder builder, Action<ShibbolethOptions> configureOptions)
            => builder.AddUWShibbolethWithHeaders(ShibbolethDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddUWShibbolethWithHeaders(this AuthenticationBuilder builder, string authenticationScheme, Action<ShibbolethOptions> configureOptions)
            => builder.AddUWShibbolethWithHeaders(authenticationScheme, displayName: ShibbolethDefaults.DisplayName, configureOptions: configureOptions);

        public static AuthenticationBuilder AddUWShibbolethWithHeaders(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ShibbolethOptions> configureOptions)
        {
            return builder.AddScheme<ShibbolethOptions, ShibbolethHeaderHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
