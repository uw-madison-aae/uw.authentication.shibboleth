using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    public class DevShibbolethAuthenticationHandler : DevAuthenticationHandler
    {
        public DevShibbolethAuthenticationHandler(IOptionsMonitor<DevAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
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

            var collection = new ShibbolethAttributeValueCollection(Options.FakeUserVariables);
            ClaimsIdentity ident = ShibbolethClaimsIdentityCreator.CreateIdentity(collection,DevAuthenticationDefaults.AuthenticationScheme);

            // add in any additional claims
            if (Options.UserClaims != null)
                ident.AddClaims(Options.UserClaims);
            
            return ident;
        }
    }
}
