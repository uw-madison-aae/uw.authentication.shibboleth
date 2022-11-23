using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Linq;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Configuration options for <see cref="ShibbolethHandler"/>.
    /// </summary>
    public class ShibbolethOptions : AuthenticationSchemeOptions
    {
        public ShibbolethOptions()
        {
            ClaimsIssuer = ShibbolethDefaults.Issuer;

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

        /// <summary>
        /// The challenge path within the application's pase bath where the ticket will be populated with Shibboleth session information
        /// </summary>
        /// <value>Local url that is secured with Shibboleth</value>
        public PathString ChallengePath { get; set; }

        /// <summary>
        /// Gets or sets whether a challenge that is initiated should be processed
        /// </summary>
        /// <remarks>If false, will produce a 401 Status Code for challenge</remarks>
        public bool ProcessChallenge { get; set; } = false;
    }

}
