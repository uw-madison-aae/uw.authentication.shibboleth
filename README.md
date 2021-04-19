# UW.Authentication

This library is for ASP.Net applications (ASP.Net 4.5+) and [Shibboleth](https://www.shibboleth.net/) authentication at the University of Wisconsin-Madison. Although the library is created using UW Shibboleth attributes, it can be overriden to utilize alternative attribute mappings for other systems.

>*The [current branch (dev)](../../tree/dev) supports .Net Core 3.1 and .Net 5 apps.*

### Purpose
- Handle MVC authentication in order to utilize things like the [Authorize] attribute
- Set HttpContext.User.Identity to a ClaimsIdentity populated with Shibboleth attributes
- Shibboleth authentication itself is handled by IIS/Apache.  This library merely consumes Shibboleth data after authentication has taken place.

### Compatibility
- ASP.Net MVC 4.5+
- IIS7+ (ISAPI or IIS7 Shibboleth DLL)

_This documentation does NOT cover Shibboleth setup for IIS/Apache.  Please refer to [UW-Madison documentation](https://kb.wisc.edu/86317) for that information._

------------
## How It Works

The library hooks into the Authentication pipeline of the ASP.Net application, examines the headers/variables for evidence of a Shibboleth session, then creates a `ClaimsPrincipal` populated with Shibboleth attributes and sets the `HttpApplication.User` with it. This is accomplished in the `HttpApplication.PostAuthenticateRequest` method in a custom `IHttpModule`

### Setup
Starting with Shibboleth SP v3, using the iis7_shib.dll with the `useVariables="true"` in either the `<ISAPI>` or `<Site>` sections is the recommended method for running Shibboleth in IIS7+.  Information about Shibboleth IIS installation [can be found here](https://wiki.shibboleth.net/confluence/display/SP3/IIS).

1. Download and install the [UW.AspNet.Authentication.Shibboleth](https://www.nuget.org/packages/UW.AspNet.Authentication.Shibboleth/) package.
2.  Add the `ShibbolethClaimsAuthenticationHttpModule` to the web.config file.

```
<system.webServer>
    <modules>
        <add name="AuthenticationModule" type="UW.AspNet.Authentication.ShibbolethClaimsAuthenticationHttpModule, UW.AspNet.Authentication.Shibboleth" />
    </modules>
</system.webServer>
```	  
------------

## Development
This package also provides with a method to "fake" authentication during development.  You can specify fake headers/variables and have them processing as it would coming from Shibboleth, or you can just specify Claims to be added to the `HttpContext.User.Identity`.

You will need to create an `HttpModule` that inherits from `UW.AspNet.Authentication.LocalDevClaimsAuthenticationHttpModule`.  In this module, you will override the `GetClaimsIdentity()` method.  Examples below include creating a `ClaimsIdentity` with manual claims, or sending a `Dictionary<string,string>` to the `ShibbolethClaimsIdentityCreator` as if they were real variables/headers.

##### Fake headers/variables
```csharp
/// <summary>
/// Module for handling development authentication - passes processing of fake header/variables to the Shibboleth library for processing
/// </summary>
public class AppShibbolethAuthenticationHttpModule : LocalDevClaimsAuthenticationHttpModule
{
    protected override ShibbolethAttributeValueCollection GetAttributesFromRequest(HttpRequest request, ShibbolethSessionType sessionType)
    {
        // create a dictionary to store "fake" headers/variables
        var variable_dict = new Dictionary<string, string>();

        variable_dict.Add("uid", "bbadger");
        variable_dict.Add("givenName", "Bucky");
        variable_dict.Add("sn", "Badger");
        variable_dict.Add("mail", "bucky.badger@wisc.edu");
        variable_dict.Add("wiscEduPVI", "UW999A999");
        variable_dict.Add("isMemberOf", "uw:domain:dept.wisc.edu:administrativestaff;uw:domain:dept.wisc.edu:it:sysadmin");

        return new ShibbolethAttributeValueCollection(variable_dict);
    }

    // You can override the GetClaimActions 
    protected override ShibbolethClaimActionCollection GetClaimActions()
    {
        var claimActions = new ShibbolethClaimActionCollection();

        claimActions.MapAttribute(StandardClaimTypes.GivenName, "givenName");
        claimActions.MapAttribute(StandardClaimTypes.Surname, "sn");
        claimActions.MapAttribute(StandardClaimTypes.PPID, "wiscEduPVI");
        claimActions.MapAttribute(StandardClaimTypes.Name, "uid");

        return claimActions;
    }
}
```
To utilize during development, put the module in your Web.config.

      <system.webServer>
        <modules>
          <!-- Used for development - if you wish to fake headers/variables and process them as Shibboleth attributes would be-->
          <add name="AuthenticationModule" type="SampleMVCNet47.AppShibbolethAuthenticationHttpModule, SampleMVCNet47"/>
        </modules>
      </system.webServer>

------------
## Customization
Several methods on the `LocalDevClaimsAuthenticationModule` or the `ShibbolethClaimsAuthenticationModule` can be overriden to allow for customization.

### Different ClaimTypes
Claims created from the Shibboleth values are done in the `GetClaimsActions` method.  An example below of overriding the method, although it is using mostly the same actions has the default method.
```csharp
protected override ShibbolethClaimActionCollection GetClaimActions()
{
    var claimActions = new ShibbolethClaimActionCollection();

    // straight claim type to attribute name mapping
    claimActions.MapAttribute(UWShibbolethClaimsType.FIRSTNAME, "givenName");
    claimActions.MapAttribute(UWShibbolethClaimsType.LASTNAME, "sn");
    claimActions.MapAttribute(UWShibbolethClaimsType.PVI, "wiscEduPVI");
    claimActions.MapAttribute(UWShibbolethClaimsType.EPPN, "eppn");

    // alter the value before it's added to the attribute
    claimActions.MapCustomAttribute(UWShibbolethClaimsType.UID, "uid", value =>
    {
        return value.ToLower();
    });

    claimActions.MapCustomAttribute(UWShibbolethClaimsType.EMAIL, "mail", value =>
    {
        return value.ToLower();
    });

    // map an attribute that has multiple values in it
    claimActions.MapCustomMultiValueAttribute(UWShibbolethClaimsType.Group, "isMemberOf", value =>
    {
        return value.Split(';').ToList();
    });

    // if you use the base.GetClaimActions, you can delete out previous added claims
    claimActions.DeleteClaim(UWShibbolethClaimsType.PVI);

    return claimActions;
}
```
------------

There are sample projects available in the source solution to show each of these working in their entirety.
 - **SampleMVCNet47** .Net MVC Framework 4.7 app using the `HttpModule` and local dev modules.
