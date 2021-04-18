using Microsoft.AspNetCore.Authentication;
using System.Linq;
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

            ClaimActions.MapAttribute(UWShibbolethClaimsType.FIRSTNAME, "givenName");
            ClaimActions.MapAttribute(UWShibbolethClaimsType.LASTNAME, "sn");
            ClaimActions.MapAttribute(UWShibbolethClaimsType.PVI, "wiscEduPVI");
            ClaimActions.MapAttribute(UWShibbolethClaimsType.EPPN, "eppn");

            ClaimActions.MapCustomAttribute(UWShibbolethClaimsType.UID, "uid", value =>
            {
                return value.ToLower();
            });

            ClaimActions.MapCustomAttribute(UWShibbolethClaimsType.EMAIL, "mail", value =>
            {
                return value.ToLower();
            });

            ClaimActions.MapCustomMultiValueAttribute(UWShibbolethClaimsType.Group, "isMemberOf", value =>
            {
                return value.Split(';').ToList();
            });
        }

        /// <summary>
        /// A collection of Shibboleth claim actions used to select values from the user data and create Claims.
        /// </summary>
        public ShibbolethClaimActionCollection ClaimActions { get; } = new ShibbolethClaimActionCollection();
    }

}
