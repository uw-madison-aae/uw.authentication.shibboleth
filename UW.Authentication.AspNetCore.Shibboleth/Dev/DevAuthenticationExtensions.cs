using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using UW.Shibboleth;

namespace UW.Authentication.AspNetCore
{
    public static class DevAuthenticationExtensions
    {
        public static AuthenticationBuilder AddDevAuthentication(this AuthenticationBuilder builder, IEnumerable<Claim> user_claims)
            => builder.AddDevAuthentication(options => { options.UserClaims = user_claims; });

        public static AuthenticationBuilder AddDevAuthentication(this AuthenticationBuilder builder, Action<DevAuthenticationOptions> configureOptions)
            => builder.AddDevAuthentication(DevAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddDevAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<DevAuthenticationOptions> configureOptions)
            => builder.AddDevAuthentication(authenticationScheme, displayName: DevAuthenticationDefaults.DisplayName, configureOptions: configureOptions);

        public static AuthenticationBuilder AddDevAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<DevAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<DevAuthenticationOptions, DevAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }

        public static AuthenticationBuilder AddDevAuthentication(this AuthenticationBuilder builder, IDictionary<string, string> fake_variables)
            => builder.AddDevShibbolethAuthentication(options => { options.FakeUserVariables = fake_variables; });

        public static AuthenticationBuilder AddDevShibbolethAuthentication(this AuthenticationBuilder builder, Action<DevAuthenticationOptions> configureOptions)
            => builder.AddDevShibbolethAuthentication(ShibbolethDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddDevShibbolethAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<DevAuthenticationOptions> configureOptions)
            => builder.AddDevShibbolethAuthentication(authenticationScheme, displayName: ShibbolethDefaults.DisplayName, configureOptions: configureOptions);

        public static AuthenticationBuilder AddDevShibbolethAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<DevAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<DevAuthenticationOptions, DevShibbolethAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
