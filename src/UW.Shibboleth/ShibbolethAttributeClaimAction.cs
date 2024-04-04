using System.Security.Claims;

namespace UW.Shibboleth;

/// <summary>
/// Infrastructure for mapping user data from a <see cref="ShibbolethAttributeValueCollection"/> to claims on the ClaimsIdentity.
/// </summary>
/// <remarks>
/// https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/OAuth/src/ClaimAction.cs
/// </remarks>
public class ShibbolethAttributeClaimAction
    : ShibbolethClaimAction
{
    /// <summary>
    /// Create a new claim manipulation action.
    /// </summary>
    /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
    /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
    /// <param name="attributeName">The Shibboleth attribute name used to extract a value from the headers/variables</param>
    public ShibbolethAttributeClaimAction(string claimType, string valueType, string attributeName)
        :base(claimType, valueType)
    {
        AttributeName = attributeName;
    }

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
    public override void Run(ShibbolethAttributeValueCollection userData, ClaimsIdentity identity, string issuer)
    {
        string? value = GetValue(userData, AttributeName);

            if (!string.IsNullOrEmpty(value))
                identity.AddClaim(new Claim(ClaimType, value, ValueType, issuer));
    }

    protected static string? GetValue(ShibbolethAttributeValueCollection userData, string attributeName) {
        if (!userData.ContainsAttribute(attributeName))
            return null;

        if (!userData.ValueIsNullOrEmpty(attributeName))
            return userData[attributeName].Value.ToString();

        return null;
    }
}
