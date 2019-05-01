using System.Web;
using UW.Shibboleth;

namespace UW.Authentication.AspNet
{
    /// <summary>
    /// Process a request containing a Shbboleth user and popoulates a ClaimsPrincipal
    /// </summary>
    /// <remarks>Shibboleth is implemented with the useVariables="true"</remarks>
    public class ShibbolethVariableHttpModule : ShibbolethClaimsAuthenticationHttpModule
    {
        public override bool IsShibbolethSession(HttpRequest request)
        {
            // must be done with the NameValueCollection because Shibboleth Server Variables don't show up in the AllKeys
            return request.ServerVariables.GetValues("Shib-Session-Index") != null;
        }
        public override ShibbolethAttributeValueCollection GetAttributesFromRequest(HttpRequest request)
        {
            return ExtractAttributes(request.ServerVariables, GetShibbolethAttributes());
        }
    }
}
