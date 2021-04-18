# UW.Authentication

This library is for ASP.Net applications (ASP.Net 5.0+) and [Shibboleth](https://www.shibboleth.net/) authentication at the University of Wisconsin-Madison. Although the library is created using UW Shibboleth attributes, it can be overriden to utilize alternative attribute mappings for other systems.

>*For those still running .Net Framework, see the [rel/2.0 branch](-/tree/rel/aspnet-2.0) for a library that uses `IHttpModule` to work with .Net Framework 4.7.2+ apps.*

### Purpose
- Handle MVC authentication in order to utilize things like the [Authorize] attribute
- Set HttpContext.User.Identity to a ClaimsIdentity populated with Shibboleth attributes
- Shibboleth authentication itself is handled by IIS/Apache.  This library merely consumes Shibboleth data after authentication has taken place.

### Compatibility
- ASP.Net Core 5.0+
- IIS7+ (ISAPI or IIS7 Shibboleth DLL)
- Apache (Front-end proxy for ASP.Net 5.0+)

_This documentation does NOT cover Shibboleth setup for IIS/Apache.  Please refer to [UW-Madison documentation](https://kb.wisc.edu/86317) for that information._

------------
## How It Works

The library hooks into the Authentication middleware of the ASP.Net application, examines the headers/variables for evidence of a Shibboleth session, then creates a `ClaimsPrincipal` populated with Shibboleth attributes and sets the `HttpApplication.User` with it. This is accomplished in a `HandleAuthenticationAsync()` method in a custom `Microsoft.AspNetCore.Authentication.AuthenticationHandler`.

### Setup

Starting with Shibboleth SP v3, using the iis7_shib.dll with the `useVariables="true"` in either the `<ISAPI>` or `<Site>` sections is the recommended method for running Shibboleth in IIS7+.  Information about Shibboleth IIS installation [can be found here](https://wiki.shibboleth.net/confluence/display/SP3/IIS).

1. Download and install the [UW.AspNetCore.Authentication.Shibboleth](https://www.nuget.org/packages/UW.AspNetCore.Authentication.Shibboleth/) package.

2.  Add the `AddUWShibboleth()` to the `IAuthenticationBuilder` in Startup.cs.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = ShibbolethAuthenticationDefaults.AuthenticationScheme;
            }).AddUWShibboleth();
        }

#### Using Apache on Linux

Headers must be forwarded from the Apache Reverse Proxy into the ASP.Net app running on Kestrel.  This is done using the `RequestHeader` declaration.  You must manually define every header from Shibboleth that you wish to use in the ASP.Net app.   **ShibSessionIndex** is **required** at a minimum, as that is what the library uses to determine if a Shibboleth session is in place.

		RequestHeader set isMemberOf %{isMemberOf}e
		RequestHeader set eppn %{eppn}e
		RequestHeader set sn %{sn}e
		RequestHeader set givenName %{givenName}e
		RequestHeader set mail %{mail}e
		RequestHeader set uid %{uid}e
		RequestHeader set wiscEduPVI %{wiscEduPVI}e
		RequestHeader set ShibSessionIndex %{ShibSessionIndex}e

------------

## Development
This package also provides with a method to "fake" authentication during development.  You can specify fake headers/variables and have them processing as it would coming from Shibboleth, or you can just specify Claims to be added to the `HttpContext.User.Identity`.

Add the `AddDevAuthentication()` method onto the `IAuthenticationBuilder`.  You can either specify a list of claims for use in `ClaimsIdentity` creation, or you can specify fake headers/variables in a `Dictionary<string,string>` that will be processing like Shibboleth headers.

##### Fake headers/variables

        // authenticate with local devauth
        // use the shibboleth processor to process these fake header/variables
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = DevAuthenticationDefaults.AuthenticationScheme;
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

There is a sample project available in the source solution to show how this works in its entirety.
 - **SampleMVCCore** .Net 5.0 app using the `IAuthenticationBuilder` extensions for both local development and Shibboleth production