using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UW.Shibboleth
{
    /// <summary>
    /// A collection of ShibbolethClaimActions used when mapping user data to Claims.
    /// </summary>
    /// <remarks>Modeled after a ClaimActionCollection
    /// https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/OAuth/src/ClaimActionCollection.cs
    /// </remarks>
    public class ShibbolethClaimActionCollection : IEnumerable<ShibbolethClaimAction>
    {
        private IList<ShibbolethClaimAction> Actions { get; } = new List<ShibbolethClaimAction>();

        /// <summary>
        /// Remove all claim actions.
        /// </summary>
        public void Clear() => Actions.Clear();

        /// <summary>
        /// Remove all claim actions for the given ClaimType.
        /// </summary>
        /// <param name="claimType">The ClaimType of maps to remove.</param>
        public void Remove(string claimType)
        {
            var itemsToRemove = Actions.Where(map => string.Equals(claimType, map.ClaimType, StringComparison.OrdinalIgnoreCase)).ToList();
            itemsToRemove.ForEach(map => Actions.Remove(map));
        }

        /// <summary>
        /// Add a claim action to the collection.
        /// </summary>
        /// <param name="action">The claim action to add.</param>
        public void Add(ShibbolethClaimAction action)
        {
            Actions.Add(action);
        }

        /// <inheritdoc />
        public IEnumerator<ShibbolethClaimAction> GetEnumerator()
        {
            return Actions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Actions.GetEnumerator();
        }
    }
}
