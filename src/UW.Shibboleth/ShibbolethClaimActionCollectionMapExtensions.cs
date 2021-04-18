using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace UW.Shibboleth
{
    /// <summary>
    /// Extension methods for <see cref="ShibbolethClaimActionCollection"/> used for mapping claim actions
    /// </summary>
    /// <remarks>
    /// Modeled after ClaimActionCollectionMapExtensions
    /// https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/OAuth/src/ClaimActionCollectionMapExtensions.cs
    /// </remarks>
    public static class ShibbolethClaimActionCollectionMapExtensions
    {
        /// <summary>
        /// Select the value from the user data with the given attribute name and add it as a Claim.
        /// This no-ops if the key is not found or the value is empty.
        /// </summary>
        /// <param name="collection">The <see cref="ShibbolethClaimActionCollection"/>.</param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="attributeName">The Shibboleth attribute name used to extract a value from the headers/variables</param>
        public static void MapAttribute(this ShibbolethClaimActionCollection collection, string claimType, string attributeName)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(ShibbolethClaimActionCollectionMapExtensions));

            collection.MapAttribute(claimType, attributeName, ClaimValueTypes.String);
        }

        /// <summary>
        /// Select the value from the user data with the given attribute name and add it as a Claim.
        /// This no-ops if the key is not found or the value is empty.
        /// </summary>
        /// <param name="collection">The <see cref="ShibbolethClaimActionCollection"/>.</param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="attributeName">The Shibboleth attribute name used to extract a value from the headers/variables</param>
        ///         /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        public static void MapAttribute(this ShibbolethClaimActionCollection collection, string claimType, string attributeName, string valueType)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(ShibbolethClaimActionCollectionMapExtensions));

            collection.Add(new ShibbolethClaimAction(claimType, valueType, attributeName));
        }


        /// <summary>
        /// Select the value from the user data with the given attribute name, process it with the given processor and add it as a Claim.
        /// This no-ops if the returned value is empty.
        /// </summary>
        /// <param name="collection">The <see cref="ShibbolethClaimActionCollection"/>.</param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="attributeName">The Shibboleth attribute name used to extract a value from the headers/variables</param>
        /// <param name="processor">The Func that will be called to processor a value from the given Shibboleth user data.</param>
        public static void MapCustomAttribute(this ShibbolethClaimActionCollection collection, string claimType, string attributeName, Func<string, string> processor)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            collection.MapCustomAttribute(claimType, attributeName, ClaimValueTypes.String, processor);
        }

        /// <summary>
        /// Select the value from the user data with the given attribute name, process it with the given processor and add it as a Claim.
        /// This no-ops if the returned value is empty.
        /// </summary>
        /// <param name="collection">The <see cref="ShibbolethClaimActionCollection"/>.</param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="attributeName">The Shibboleth attribute name used to extract a value from the headers/variables</param>
        /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        /// <param name="processor">The Func that will be called to processor a value from the given Shibboleth user data.</param>
        public static void MapCustomAttribute(this ShibbolethClaimActionCollection collection, string claimType, string attributeName, string valueType, Func<string, string> processor)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            collection.Add(new ShibbolethCustomClaimAction(claimType, valueType, attributeName, processor));
        }
        /// <summary>
        /// Select the value from the user data with the given attribute name, process it with the given processor and add the multiple return values as a Claim.
        /// This no-ops if the returned value is empty.
        /// </summary>
        /// <param name="collection">The <see cref="ShibbolethClaimActionCollection"/>.</param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="attributeName">The Shibboleth attribute name used to extract a value from the headers/variables</param>
        /// <param name="processor">The Func that will be called to processor a value from the given Shibboleth user data.</param>
        public static void MapCustomMultiValueAttribute(this ShibbolethClaimActionCollection collection, string claimType, string attributeName, Func<string, IEnumerable<string>> processor)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            collection.MapCustomMultiValueAttribute(claimType, attributeName, ClaimValueTypes.String, processor);
        }

        /// <summary>
        /// Select the value from the user data with the given attribute name, process it with the given processor and add the multiple return values as a Claim.
        /// This no-ops if the returned value is empty.
        /// </summary>
        /// <param name="collection">The <see cref="ShibbolethClaimActionCollection"/>.</param>
        /// <param name="claimType">The value to use for Claim.Type when creating a Claim.</param>
        /// <param name="attributeName">The Shibboleth attribute name used to extract a value from the headers/variables</param>
        /// <param name="valueType">The value to use for Claim.ValueType when creating a Claim.</param>
        /// <param name="processor">The Func that will be called to processor a value from the given Shibboleth user data.</param>
        public static void MapCustomMultiValueAttribute(this ShibbolethClaimActionCollection collection, string claimType, string attributeName, string valueType, Func<string, IEnumerable<string>> processor)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            collection.Add(new ShibbolethCustomMultiValueClaimAction(claimType, valueType, attributeName, processor));
        }
    }
}
