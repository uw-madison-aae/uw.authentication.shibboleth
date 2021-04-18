using System.Security.Claims;

namespace UW.Shibboleth
{
    /// <summary>
    /// Creates a <see cref="ClaimsIdentity"/> loaded with the minimum Shibboleth attributes
    /// </summary>
    /// <remarks>Adds uid, givenName, sn, isMemberOf (groups), mail, eppn, and wiscEduPVI</remarks>
    public static class ShibbolethClaimsIdentityCreator
    {
        public static ClaimsIdentity CreateIdentity(ShibbolethAttributeValueCollection collection, string authenticationType)
        {
            ClaimsIdentity ident = new ClaimsIdentity(authenticationType);

            if (collection.ContainsId("uid") && !collection.ValueIsNullOrEmpty("uid")) ident.AddClaim(new Claim(UWShibbolethClaimsType.UID, collection["uid"].Value.ToString().ToLower()));
            if (collection.ContainsId("givenName") && !collection.ValueIsNullOrEmpty("givenName")) ident.AddClaim(new Claim(UWShibbolethClaimsType.FIRSTNAME, collection["givenName"].Value));
            if (collection.ContainsId("sn") && !collection.ValueIsNullOrEmpty("sn")) ident.AddClaim(new Claim(UWShibbolethClaimsType.LASTNAME, collection["sn"].Value));
            if (collection.ContainsId("mail") && !collection.ValueIsNullOrEmpty("mail")) ident.AddClaim(new Claim(UWShibbolethClaimsType.EMAIL, collection["mail"].Value.ToString().ToLower()));
            if (collection.ContainsId("wiscEduPVI") && !collection.ValueIsNullOrEmpty("wiscEduPVI")) ident.AddClaim(new Claim(UWShibbolethClaimsType.PVI, collection["wiscEduPVI"].Value));
            if (collection.ContainsId("eppn") && !collection.ValueIsNullOrEmpty("eppn")) ident.AddClaim(new Claim(UWShibbolethClaimsType.EPPN, collection["eppn"].Value));

            if (collection.ContainsId("isMemberOf") && !collection.ValueIsNullOrEmpty("isMemberOf"))
            {
                string[] memberOf = collection["isMemberOf"].Value.ToString().Split(';');
                foreach (string member in memberOf)
                {
                    ident.AddClaim(new Claim(UWShibbolethClaimsType.Group, member));
                }
            }

            return ident;
        }

        /// <summary>
        /// Creates a <see cref="ClaimsIdentity"/> populated with the 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static ClaimsIdentity CreateIdentity(ShibbolethAttributeValueCollection collection)
        {
            return CreateIdentity(collection, ShibbolethAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
