using System.Web;
using UW.Shibboleth;

namespace UW.AspNet.Authentication
{
    /// <summary>
    /// Local development claims - must be overridden in each app to manually specify a user/claims
    /// </summary>
    public abstract class LocalDevClaimsAuthenticationHttpModule : ShibbolethClaimsAuthenticationHttpModule
    {
        protected override ShibbolethAttributeValueCollection GetAttributesFromRequest(HttpRequest request, ShibbolethSessionType sessionType)
        {
            return base.GetAttributesFromRequest(request, sessionType);
        }

        protected override ShibbolethSessionType IsShibbolethSession(HttpRequest request)
        {
            // return something other than None
            return ShibbolethSessionType.Variable;
        }
    }
}
