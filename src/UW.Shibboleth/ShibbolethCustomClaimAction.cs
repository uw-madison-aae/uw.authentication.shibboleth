using System.Security.Claims;

namespace UW.Shibboleth;

/// <summary>
/// ShibbolethClaimAction that processes the value from the user data after extraction by running the given Func processor
/// </summary>
public class ShibbolethCustomClaimAction : ShibbolethAttributeClaimAction
{
    /// <summary>
    /// Creates a new <see cref="ShibbolethCustomClaimAction"/>
    /// </summary>
    /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
    /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
    /// <param name="attributeName">The Shibboleth attribute name used to extract a value from the headers/variables</param>
    /// <param name="processor">Func to process a Shibboleth value</param>
    public ShibbolethCustomClaimAction(string claimType, string valueType, string attributeName, Func<string, string?> processor)
        :base(claimType,valueType,attributeName)
    {
        Processor = processor;
    }

    /// <summary>
    /// The Func that will be called to processor a value from the given Shibboleth user data
    /// </summary>
    public Func<string, string?> Processor { get; }

    public override void Run(ShibbolethAttributeValueCollection userData, ClaimsIdentity identity, string issuer)
    {
        string? value = GetValue(userData, AttributeName);

        if (!string.IsNullOrEmpty(value))
        {
            string? processedValue = Processor(value!);
            if (!string.IsNullOrEmpty(processedValue))
                identity.AddClaim(new Claim(ClaimType, processedValue, ValueType, issuer));
        }
    }
}
