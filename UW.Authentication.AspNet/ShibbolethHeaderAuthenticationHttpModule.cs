using System.Web;
using UW.Shibboleth;

namespace UW.Authentication.AspNet
{
    /// <summary>
    /// Process a request containing a Shbboleth user and popoulates a ClaimsPrincipal with data from the headers
    /// </summary>
    /// <remarks>Shibboleth is implemented with the useHeaders="true" or is using the isapi_shib.dll</remarks>
    public class ShibbolethHeaderAuthenticationHttpModule : ShibbolethClaimsAuthenticationHttpModule
    {
        protected override bool IsShibbolethSession(HttpRequest request)
        {
            return request.Headers.GetValues("ShibSessionIndex") != null;
        }
        protected override ShibbolethAttributeValueCollection GetAttributesFromRequest(HttpRequest request)
        {
            return ShibbolethAttributeExtractor.ExtractAttributes(request.Headers, GetShibbolethAttributes());
        }
    }
}
