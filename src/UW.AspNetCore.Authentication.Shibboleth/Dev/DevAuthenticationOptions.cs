using System.Collections.Generic;
using System.Security.Claims;

namespace UW.AspNetCore.Authentication
{
    public class DevAuthenticationOptions : ShibbolethAuthenticationOptions
    {
        public DevAuthenticationOptions() : base()
        {
            UserClaims = null;
            FakeUserVariables = null;
        }
        public DevAuthenticationOptions(IEnumerable<Claim> user_claims) : this()
        {
            UserClaims = user_claims;
        }

        public DevAuthenticationOptions(IDictionary<string, string> fake_variables) : this()
        {
            FakeUserVariables = fake_variables;
        }

        public IEnumerable<Claim> UserClaims { get; set; }
        public IDictionary<string, string> FakeUserVariables { get; set; }
    }

}
