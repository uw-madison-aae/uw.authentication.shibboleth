# UW.Authentication

This library is for ASP.Net applications (ASP.Net Core 2.0+, ASP.Net 4.5+) and [Shibboleth](https://www.shibboleth.net/) authentication at the University of Wisconsin-Madison. Although the library is created using UW Shibboleth attributes, it can be overriden to utilize alternative attribute mappings for other systems.

### Purpose
- Handle MVC authentication in order to utilize things like the [Authorize] attribute
- Set HttpContext.User.Identity to a ClaimsIdentity populated with Shibboleth attributes
- Shibboleth authentication itself is handled by IIS/Apache.  This library merely consumes Shibboleth data after authentication has taken place.

### Compatibility
- ASP.Net Core 2.0+
- ASP.Net MVC 4.5+
- IIS7+ (ISAPI or IIS7 Shibboleth DLL)
- Apache (Front-end proxy for ASP.Net Core 2.0+)

------------
## How It Works

The library hooks into the Authentication pipeline of the ASP.Net application, examines the headers/variables for evidence of a Shibboleth session, then creates a `ClaimsPrincipal` populated with Shibboleth attributes and sets the `HttpApplication.User` with it.
 - For ASP.Net Core 2.0+ this is accomplished in a `HandleAuthenticationAsync()` method in a custom `Microsoft.AspNetCore.Authentication.AuthenticationHandler`
- For ASP.Net MVC 4/5 this is accomplished in the `HttpApplication.PostAuthenticateRequest` method in a custom `IHttpModule`

### ASP.Net MVC 4/5 Setup
#### Using iis7_shib.dll (Recommended)
Starting with Shibboleth SP v3, using the iis7_shib.dll with the `useVariables="true"` in either the `<ISAPI>` or `<Site>` sections is the recommended method for running Shibboleth in IIS7+.  Information about Shibboleth IIS installation [can be found here](https://wiki.shibboleth.net/confluence/display/SP3/IIS).

1. Download and install the [UW.AspNet.Authentication.Shibboleth](https://www.nuget.org/packages/UW.AspNet.Authentication.Shibboleth/) package.
2.  Add the `ShibbolethVariableHttpModule` to the web.config file.

```
<system.webServer>
    <modules>
        <add name="AuthenticationModule" type="UW.AspNet.Authentication.ShibbolethVariableHttpModule, UW.AspNet.Authentication" />
    </modules>
</system.webServer>
```	  
#### Using isapi_shib.dll (or iis7_shib with useHeaders="true")
This is **NOT** recommended by the authors of Shibboleth.  The server variables method in iis7_shib.dll is more secure.

1. Download and install [UW.Authentication.AspNet.Shibboleth](https://www.nuget.org/packages/UW.Authentication.AspNet.Shibboleth/) package.
2. Add the `ShibbolethHeaderHttpModule` to the web.config file.

```
<system.webServer>
    <modules>
        <add name="AuthenticationModule" type="UW.AspNet.Authentication.ShibbolethHeaderHttpModule, UW.AspNet.Authentication" />
    </modules>
</system.webServer>
```

|   | **IIS (isapi_shib)** | **IIS (iis7_shib)**<br>_useVariables="true"_ | **IIS (iis7_shib)**<br>_useHeaders="true"_ | **Apache** |
| --- | --- | --- | --- | ---|
|  ASP.Net Core 2.0+ | ShibbolethHeaderHandler<br>_OutOfProcess or InProcess_  | ShibbolethVariableHandler<br>_InProcess only_  | ShibbolethHeaderHandler<br>_OutOfProcess or InProcess_ | ShibbolethHeaderHandler |
| ASP.NET MVC 4/5 | ShibbolethHeaderHttpModule | ShibbolethVariableHttpModule | ShibbolethHeaderHttpModule | N/A |

### ASP.Net Core 2.0+ Setup

#### Using IIS - iis7_shib.dll (Recommended)
Starting with Shibboleth SP v3, using the iis7_shib.dll with the `useVariables="true"` in either the `<ISAPI>` or `<Site>` sections is the recommended method for running Shibboleth in IIS7+.  Information about Shibboleth IIS installation [can be found here](https://wiki.shibboleth.net/confluence/display/SP3/IIS).

1. Download and install the [UW.AspNetCore.Authentication.Shibboleth](https://www.nuget.org/packages/UW.AspNetCore.Authentication.Shibboleth//) package.
2.  Add the `AddUWShibbolethForIISWithVariables()` to the `IAuthenticationBuilder` in Startup.cs.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
            }).AddUWShibbolethForIISWithVariables();
        }

#### Using IIS - isapi_shib.dll (or iis7_shib with useHeaders="true")
This is **NOT** recommended by the authors of Shibboleth.  The server variables method in iis7_shib.dll is more secure.

1. Download and install the [UW.AspNetCore.Authentication.Shibboleth](https://www.nuget.org/packages/UW.AspNetCore.Authentication.Shibboleth//) package.
2.  Add the `AddUWShibbolethForIISWithHeaders()` to the `IAuthenticationBuilder` in Startup.cs.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
            }).AddUWShibbolethForIISWithHeaders();
        }

#### Using IIS - isapi_shib.dll (or iis7_shib with useHeaders="true")
This is **NOT** recommended by the authors of Shibboleth.  The server variables method in iis7_shib.dll is more secure.

1. Download and install the [UW.AspNetCore.Authentication.Shibboleth](https://www.nuget.org/packages/UW.AspNetCore.Authentication.Shibboleth//) package.
2.  Add the `AddUWShibbolethForIISWithHeaders()` to the `IAuthenticationBuilder` in Startup.cs.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
            }).AddUWShibbolethForIISWithHeaders();
        }

#### Using Apache on Linux

1. Download and install the [UW.AspNetCore.Authentication.Shibboleth](https://www.nuget.org/packages/UW.AspNetCore.Authentication.Shibboleth//) package.
2.  Add the `AddUWShibbolethForLinux()` to the `IAuthenticationBuilder` in Startup.cs.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
            }).AddUWShibbolethForLinux();
        }


------------

## Development
This package also provides with a method to "fake" authentication during development.  You can specify fake headers/variables and have them processing as it would coming from Shibboleth, or you can just specify Claims to be added to the `HttpContext.User.Identity`.

### ASP.Net MVC 4/5
You will need to create an `HttpModule` that inherits from `UW.AspNet.Authentication.LocalDevClaimsAuthenticationHttpModule`.  In this module, you will override the `GetClaimsIdentity()` method.  Examples below include creating a `ClaimsIdentity` with manual claims, or sending a `Dictionary<string,string>` to the `ShibbolethClaimsIdentityCreator` as if they were real variables/headers.

##### Fake headers/variables
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

##### Manual claims creation
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

To utilize during development, put the module in your Web.config.

      <system.webServer>
        <modules>
          <!-- Used for development - if you wish to fake headers/variables and process them as Shibboleth attributes would be-->
          <add name="AuthenticationModule" type="SampleMVCNet47.AppShibbolethAuthenticationHttpModule, SampleMVCNet47"/>
        </modules>
      </system.webServer>

### ASP.Net Core 2.0+
Add the `AddDevAuthentiction()` method onto the `IAuthenticationBuilder`.  You can either specify a list of claims for use in `ClaimsIdentity` creation, or you can specify fake headers/variables in a `Dictionary<string,string>` that will be processing like Shibboleth headers.

##### Fake headers/variables

        // authenticate with local devauth
        // use the shibboleth processor to process these fake header/variables
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
        }).AddDevAuthentication(new Dictionary<string, string>() {
            {"uid", "bbadger" },
            {"givenName", "Bucky" },
            {"sn", "Badger"},
            {"mail", "bucky.badger@wisc.edu" },
            {"wiscEduPVI", "UW999A999" },
            {"isMemberOf", "uw:domain:dept.wisc.edu:administrativestaff;uw:domain:dept.wisc.edu:it:sysadmin" }
        });

##### Manual claim creation

        // authenticate with local devauth
        // put in the specific claims you want 
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = DevAuthenticationDefaults.AuthenticationScheme;
        }).AddDevAuthentication(new List<Claim>() {
            new Claim(StandardClaimTypes.Name, "bbadger"),
            new Claim(StandardClaimTypes.GivenName, "Bucky"),
            new Claim(StandardClaimTypes.Surname, "Badger"),
            new Claim(StandardClaimTypes.PPID, "UW999A999"),
            new Claim(StandardClaimTypes.Email, "bucky.badger@wisc.edu"),
            new Claim(StandardClaimTypes.Group, "uw:domain:dept.wisc.edu:administrativestaff"),
            new Claim(StandardClaimTypes.Group, "uw:domain:dept.wisc.edu:it:sysadmin"),
        });


------------

There are sample projects available in the source solution to show each of these working in their entirety.
 - **SampleMVCNet47** .Net MVC Framework 4.7 app using the `HttpModule` and local dev modules.

 - **SampleMVCCore** .Net Core 2.2 app using the `IAuthenticationBuilder` extensions for both local development and Shibboleth production