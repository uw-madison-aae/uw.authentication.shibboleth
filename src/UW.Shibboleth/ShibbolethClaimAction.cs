using System.Security.Claims;   

namespace UW.Shibboleth
{
    /// <summary>
    /// Infrastructure for mapping user data from a <see cref="ShibbolethAttributeValueCollection"/> to claims on the ClaimsIdentity.
    /// </summary>
    /// https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/OAuth/src/ClaimAction.cs
    /// </remarks>
    public class ShibbolethClaimAction
    {
        /// <summary>
        /// Create a new claim manipulation action.
        /// </summary>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        /// <param name="attributeName">The Shibboleth attribute name used to extract a value from the headers/variables</param>
        public ShibbolethClaimAction(string claimType, string valueType, string attributeName)
        {
            ClaimType = claimType;
            ValueType = valueType;
            AttributeName = attributeName;
        }

        /// <summary>
        /// Gets the value to use for <see cref="Claim.Value"/>when creating a Claim.
        /// </summary>
        public string ClaimType { get; }

        /// <summary>
        /// Gets the value to use for <see cref="Claim.ValueType"/> when creating a Claim. 
        /// </summary>
        public string ValueType { get; }

        /// <summary>
        /// Gets the value to use retrieving the Shibboleth attribute value when created a Claim
        /// </summary>
        public string AttributeName { get; }

        /// <summary>
        /// Examine the given <see cref="ShibbolethAttributeValueCollection"/>, determine if the requisite data is present, and optionally add it
        /// as a new Claim on the ClaimsIdentity.
        /// </summary>
        /// <param name="userData">The source data to examine. This value may be null.</param>
        /// <param name="identity">The identity to add Claims to.</param>
        /// <param name="issuer">The value to use for Claim.Issuer when creating a Claim.</param>
        public virtual void Run(ShibbolethAttributeValueCollection userData, ClaimsIdentity identity, string issuer)
        {
                var value = GetValue(userData, AttributeName);

                if (!string.IsNullOrEmpty(value))
                    identity.AddClaim(new Claim(ClaimType, value, ValueType, issuer));
        }

        protected static string GetValue(ShibbolethAttributeValueCollection userData, string attributeName) {
            if (!userData.ContainsId(attributeName))
                return null;

            if (!userData.ValueIsNullOrEmpty(attributeName))
                return userData[attributeName].Value.ToString();

            return null;
        }
    }
}
