using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;

namespace UW.Authentication.AspNetCore
{
    public class DevAuthenticationOptions : AuthenticationSchemeOptions
    {
        public DevAuthenticationOptions()
        {

        }
        public DevAuthenticationOptions(IEnumerable<Claim> user_claims)
        {
            UserClaims = user_claims;
        }

        public IEnumerable<Claim> UserClaims { get; set; }
    }
}
