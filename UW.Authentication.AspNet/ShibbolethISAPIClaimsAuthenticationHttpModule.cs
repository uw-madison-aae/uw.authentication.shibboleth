using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UW.Identity;

namespace UW.Authentication.AspNet
{
    /// <summary>
    /// Process a request containing a Shbboleth user and popoulates a ClaimsPrincipal
    /// </summary>
    /// <remarks>Shibboleth is implemented using the isapi_shib.dll module</remarks>
    public class ShibbolethISAPIClaimsAuthenticationHttpModule : ShibbolethClaimsAuthenticationHttpModule
    {
        protected override IDictionary<string, string> GetAttributes(NameValueCollection collection)
        {
            throw new NotImplementedException();
        }

        protected override bool IsShibbolethSession(NameValueCollection collection)
        {
            return collection.GetValues("ShibSessionIndex") != null;
        }
    }
}
