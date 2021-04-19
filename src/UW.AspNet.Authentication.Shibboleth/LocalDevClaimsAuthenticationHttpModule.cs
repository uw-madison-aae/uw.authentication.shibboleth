using System.Web;
using UW.Shibboleth;

namespace UW.AspNet.Authentication
{
    /// <summary>
    /// Local development claims - must be overridden in each app to manually specify a user/claims
    /// </summary>
    public abstract class LocalDevClaimsAuthenticationHttpModule : ShibbolethClaimsAuthenticationHttpModule
    {
        protected abstract override ShibbolethAttributeValueCollection GetAttributesFromRequest(HttpRequest request, ShibbolethSessionType sessionType);

        protected override ShibbolethSessionType IsShibbolethSession(HttpRequest request)
        {
            // return something other than None
            return ShibbolethSessionType.Variable;
        }
    }
}
