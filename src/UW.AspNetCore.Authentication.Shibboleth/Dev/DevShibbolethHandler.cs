using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    public class DevShibbolethHandler : DevAuthenticationHandler
    {
        public DevShibbolethHandler(IOptionsMonitor<DevAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

        }

        protected override ClaimsIdentity CreateUserIdentity()
        {
            // create a Shibboleth-type user
            // create a ShibbolethValueCollection from the fake headers/variables

            if (Options.FakeUserVariables == null)
            {
                throw new System.Exception("No user variables/headers specified for Dev Shibboleth authentication.");
            }

            var userData = new ShibbolethAttributeValueCollection(Options.FakeUserVariables);
            var identity = new ClaimsIdentity(Scheme.Name);

            // examine the specified user data, determine if requisite data is present, and optionally add it
            foreach (var action in Options.ClaimActions)
            {
                action.Run(userData, identity, Options.ClaimsIssuer ?? Scheme.Name);
            }

            // add in any additional claims
            if (Options.UserClaims != null)
                identity.AddClaims(Options.UserClaims);

            return identity;

        }
    }
}
