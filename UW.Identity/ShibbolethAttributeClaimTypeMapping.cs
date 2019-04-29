using System;
using System.Collections.Generic;
using System.Text;

namespace UW.Identity
{
    /// <summary>
    /// Class for mapping Shibboleth attributes to ClaimTypes
    /// </summary>
    public class ShibbolethAttributeClaimTypeMapping
    {
        /// <param name="attribute">Shibboleth attribute Id (givenName, sn, etc) </param>
        /// <param name="claimType">ClaimType (http://schemas.xmlsoap.org/ws/2009/09/identity/claims/id)</param>
        public ShibbolethAttributeClaimTypeMapping(string attributeId, string claimType)
        {
            ShibbolethAttributeId = attributeId;
            ClaimType = claimType;
        }

        public string ShibbolethAttributeId { get; }
        public string ClaimType { get; }
    }
}
