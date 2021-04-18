using System.Linq;
using System.Security.Claims;

namespace UW.Shibboleth
{
    /// <summary>
    /// A ShibbolethClaimAction that deletes all claims from the given ClaimsIdentity with the given ClaimType.
    /// </summary>
    public class ShibbolethDeleteClaimAction : ShibbolethClaimAction
    {
        /// <summary>
        /// Creates a new <see cref="ShibbolethDeleteClaimAction"/>
        /// </summary>
        /// <param name="claimType">The ClaimType of Claims to delete</param>
        public ShibbolethDeleteClaimAction(string claimType)
            :base(claimType, ClaimValueTypes.String)
        {

        }

        /// <inheritdoc />
        public override void Run(ShibbolethAttributeValueCollection userData, ClaimsIdentity identity, string issuer)
        {
            foreach (var claim in identity.FindAll(ClaimType).ToList())
            {
                identity.TryRemoveClaim(claim);
            }
        }
    }
}
