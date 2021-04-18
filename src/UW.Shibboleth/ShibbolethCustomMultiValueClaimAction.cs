using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace UW.Shibboleth
{
    /// <summary>
    /// ShibbolethClaimAction that processes the value from the user data after extraction by running the given Func processor, returning multiple values
    /// </summary>
    public class ShibbolethCustomMultiValueClaimAction : ShibbolethClaimAction
    {
        /// <summary>
        /// Creates a new <see cref="ShibbolethCustomMultiValueClaimAction"/>
        /// </summary>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        /// <param name="attributeName">The Shibboleth attribute name used to extract a value from the headers/variables</param>
        public ShibbolethCustomMultiValueClaimAction(string claimType, string valueType, string attributeName, Func<string, IEnumerable<string>> processor)
            :base(claimType,valueType,attributeName)
        {
            Processor = processor;
        }

        /// <summary>
        /// The Func that will be called to processor a value from the given Shibboleth user data
        /// </summary>
        public Func<string, IEnumerable<string>> Processor { get; }

        public override void Run(ShibbolethAttributeValueCollection userData, ClaimsIdentity identity, string issuer)
        {
            var value = GetValue(userData, AttributeName);

            if (!string.IsNullOrEmpty(value))
            {
                var processedValues = Processor(value);
                foreach(var processedValue in processedValues)      
                {
                    if (!string.IsNullOrEmpty(processedValue))
                        identity.AddClaim(new Claim(ClaimType, processedValue, ValueType, issuer));
                }
                
            }
                
        }
    }
}
