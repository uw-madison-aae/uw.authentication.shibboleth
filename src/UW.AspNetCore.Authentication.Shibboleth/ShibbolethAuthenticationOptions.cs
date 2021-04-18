using Microsoft.AspNetCore.Authentication;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Configuration options for <see cref="ShibbolethAuthenticationHandler"/>.
    /// </summary>
    public class ShibbolethAuthenticationOptions : AuthenticationSchemeOptions
    {
        public ShibbolethAuthenticationOptions()
        {
            ClaimsIssuer = ShibbolethAuthenticationDefaults.Issuer;
        }   
    }

}
