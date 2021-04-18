using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace UW.AspNetCore.Authentication
{
    public class DevAuthenticationHandler : AuthenticationHandler<DevAuthenticationOptions>
    {
        public DevAuthenticationHandler(IOptionsMonitor<DevAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

        }


        /// <summary>
        /// Uses <see cref="DevAuthenticationOptions"/> to see the user
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            ClaimsIdentity ident = CreateUserIdentity();

            ClaimsPrincipal user = new ClaimsPrincipal(ident);

            return Task.FromResult(
                 AuthenticateResult.Success(
                    new AuthenticationTicket(
                        //new ClaimsPrincipal(Options.Identity),
                        //new ClaimsPrincipal(),
                        user,
                        new AuthenticationProperties(),
                        this.Scheme.Name)));
        }

        protected virtual ClaimsIdentity CreateUserIdentity()
        {
            ClaimsIdentity ident = new ClaimsIdentity(DevAuthenticationDefaults.AuthenticationScheme);
            if (Options.UserClaims != null)
                ident.AddClaims(Options.UserClaims);
            return ident;
        }
    }
}
