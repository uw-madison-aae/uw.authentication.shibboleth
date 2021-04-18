using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;

namespace UW.AspNetCore.Authentication
{
    public class DevAuthenticationOptions : AuthenticationSchemeOptions
    {
        public DevAuthenticationOptions()
        {
            UserClaims = null;
            FakeUserVariables = null;
        }
        public DevAuthenticationOptions(IEnumerable<Claim> user_claims)
        {
            UserClaims = user_claims;
        }

        public DevAuthenticationOptions(IDictionary<string, string> fake_variables)
        {
            FakeUserVariables = fake_variables;
        }

        public IEnumerable<Claim> UserClaims { get; set; }
        public IDictionary<string, string> FakeUserVariables {get;set;}
    }
}
