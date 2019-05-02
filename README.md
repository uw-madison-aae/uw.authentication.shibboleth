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

1. Download and install the [UW.Authentication.AspNet.Shibboleth](https://www.nuget.org/packages/UW.Authentication.AspNet.Shibboleth/) package.
2.  Add the `ShibbolethVariableHttpModule` to the web.config file.

```
<system.webServer>
    <modules>
        <add name="AuthenticationModule" type="UW.Authentication.AspNet.ShibbolethVariableHttpModule, UW.Authentication.AspNet" />
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
        <add name="AuthenticationModule" type="UW.Authentication.AspNet.ShibbolethHeaderHttpModule, UW.Authentication.AspNet" />
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

1. Download and install the [UW.Authentication.AspNetCore.Shibboleth](https://www.nuget.org/packages/UW.Authentication.AspNetCore.Shibboleth//) package.
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

1. Download and install the [UW.Authentication.AspNetCore.Shibboleth](https://www.nuget.org/packages/UW.Authentication.AspNetCore.Shibboleth//) package.
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

1. Download and install the [UW.Authentication.AspNetCore.Shibboleth](https://www.nuget.org/packages/UW.Authentication.AspNetCore.Shibboleth//) package.
2.  Add the `AddUWShibbolethForIISWithHeaders()` to the `IAuthenticationBuilder` in Startup.cs.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
            }).AddUWShibbolethForIISWithHeaders();
        }

#### Using Apache on Linux

1. Download and install the [UW.Authentication.AspNetCore.Shibboleth](https://www.nuget.org/packages/UW.Authentication.AspNetCore.Shibboleth//) package.
2.  Add the `AddUWShibbolethForLinux()` to the `IAuthenticationBuilder` in Startup.cs.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
            }).AddUWShibbolethForLinux();
        }