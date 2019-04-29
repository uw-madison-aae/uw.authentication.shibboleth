using System.Collections.Specialized;

namespace UW.Authentication.AspNet
{
    /// <summary>
    /// Process a request containing a Shbboleth user and popoulates a ClaimsPrincipal
    /// </summary>
    /// <remarks>Shibboleth is implemented with the useVariables="true"</remarks>
    public class ShibbolethVariableAuthenticationHttpModule : ShibbolethClaimsAuthenticationHttpModule
    {
        protected override bool IsShibbolethSession(NameValueCollection collection)
        {
            // must be done with the NameValueCollection because Shibboleth Server Variables don't show up in the AllKeys
            return collection.GetValues("Shib-Session-Index") != null;
        }
    }
}
