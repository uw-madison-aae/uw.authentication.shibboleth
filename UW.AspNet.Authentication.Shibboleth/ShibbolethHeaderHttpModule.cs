using System.Web;
using UW.Shibboleth;

namespace UW.AspNet.Authentication
{
    /// <summary>
    /// Process a request containing a Shbboleth user and popoulates a ClaimsPrincipal with data from the headers
    /// </summary>
    /// <remarks>Shibboleth is implemented with the useHeaders="true" or is using the isapi_shib.dll</remarks>
    public class ShibbolethHeaderHttpModule : ShibbolethClaimsAuthenticationHttpModule
    {
        public override bool IsShibbolethSession(HttpRequest request)
        {
            return request.Headers.GetValues("ShibSessionIndex") != null;
        }
        public override ShibbolethAttributeValueCollection GetAttributesFromRequest(HttpRequest request)
        {
            return ExtractAttributes(request.Headers, GetShibbolethAttributes());
        }
    }
}
