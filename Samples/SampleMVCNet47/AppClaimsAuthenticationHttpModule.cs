using System.Collections.Generic;
using System.Security.Claims;
using UW.Authentication.AspNet;
using UW.Shibboleth;

namespace SampleMVCNet47
{
    /// <summary>
    /// Module for handling development authentication - creates claims
    /// </summary>
    public class AppClaimsAuthenticationHttpModule : LocalDevClaimsAuthenticationHttpModule
    {


        // provide a ClaimsIdentity to the module to fake authentication
        public override ClaimsIdentity GetClaimsIdentity()
        {
            // create a fake Shibboleth authenticated identity
            var ident = new ClaimsIdentity(DevAuthenticationDefaults.AuthenticationScheme);

            ident.AddClaims(new List<Claim>() {
                    new Claim(StandardClaimTypes.Name, "bbadgers"),
                    new Claim(StandardClaimTypes.GivenName, "Bucky"),
                    new Claim(StandardClaimTypes.Surname, "Badger"),
                    new Claim(StandardClaimTypes.PPID, "UW999A999"),        // wiscEduPVI
                    new Claim(StandardClaimTypes.Email, "bucky.badger@wisc.edu"),
                    new Claim(StandardClaimTypes.Group, "uw:domain:dept.wisc.edu:administrativestaff"),
                    new Claim(StandardClaimTypes.Group, "uw:domain:dept.wisc.edu:it:sysadmin"),
                });

            return ident;
        }

    }
}