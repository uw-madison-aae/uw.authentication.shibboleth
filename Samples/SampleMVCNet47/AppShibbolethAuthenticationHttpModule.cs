﻿using System.Collections.Generic;
using System.Security.Claims;
using UW.AspNet.Authentication;
using UW.Shibboleth;

namespace SampleMVCNet47
{
    /// <summary>
    /// Module for handling development authentication - passes processing of fake header/variables to the Shibboleth library for processing
    /// </summary>
    public class AppShibbolethAuthenticationHttpModule : LocalDevClaimsAuthenticationHttpModule
    {
        // provide a ClaimsIdentity to the module to fake authentication
        public override ClaimsIdentity GetClaimsIdentity()
        {
            // create a dictionary to store "fake" headers/variables
            var variable_dict = new Dictionary<string, string>();

            variable_dict.Add("uid", "bbadger");
            variable_dict.Add("givenName", "Bucky");
            variable_dict.Add("sn", "Badger");
            variable_dict.Add("mail", "bucky.badger@wisc.edu");
            variable_dict.Add("wiscEduPVI", "UW999A999");
            variable_dict.Add("isMemberOf", "uw:domain:dept.wisc.edu:administrativestaff;uw:domain:dept.wisc.edu:it:sysadmin");

            // Create a ShibbolethAttributeValueCollection with the fake headers/variables, then use the ShibbolethClaimsIdentityCreatetor to make the ClaimsIdentity            
            return ShibbolethClaimsIdentityCreator.CreateIdentity(new ShibbolethAttributeValueCollection(variable_dict));
        }

    }
}