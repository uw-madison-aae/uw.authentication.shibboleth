# UW.Authentication

This library is for ASP.NET applications (ASP.Net 3.1/5/6+) and [Shibboleth](https://www.shibboleth.net/) authentication at the University of Wisconsin-Madison. Although the library is created using UW Shibboleth attributes, it can be overriden to utilize alternative attribute mappings for other systems.

>*For those still running .Net Framework, see the [rel/2.0 branch](../../tree/rel/aspnet-2.0) for a library that uses `IHttpModule` to work with .Net Framework 4.6.2+ apps.*

### Purpose
- Handle MVC authentication in order to utilize things like the [Authorize] attribute
- Set HttpContext.User.Identity to a ClaimsIdentity populated with Shibboleth attributes
- Shibboleth authentication itself is handled by IIS/Apache.  This library merely consumes Shibboleth data after authentication has taken place.

### Compatibility
- ASP.NET 3.1/5/6+
- IIS7+ (ISAPI or IIS7 Shibboleth DLL)
- Apache (Front-end proxy for ASP.NET 3.1/5/6+)

_This documentation does NOT cover Shibboleth setup for IIS/Apache.  Please refer to [UW-Madison documentation](https://kb.wisc.edu/86317) for that information._

------------
## How It Works

The library hooks into the Authentication middleware of the ASP.NET application, examines the headers/variables for evidence of a Shibboleth session, then creates a `ClaimsPrincipal` populated with Shibboleth attributes and sets the `HttpApplication.User` with it. This is accomplished in a `HandleAuthenticationAsync()` method in a custom `Microsoft.AspNetCore.Authentication.AuthenticationHandler`.

# Setup

Starting with Shibboleth SP v3, using the iis7_shib.dll with the `useVariables="true"` in either the `<ISAPI>` or `<Site>` sections is the recommended method for running Shibboleth in IIS7+.  Information about Shibboleth IIS installation [can be found here](https://wiki.shibboleth.net/confluence/display/SP3/IIS).

1. Download and install the [UW.AspNetCore.Authentication.Shibboleth](https://www.nuget.org/packages/UW.AspNetCore.Authentication.Shibboleth/) package.

2.  Add the `AddUWShibboleth()` to the `IAuthenticationBuilder` in Program.cs (.NET6) or Startup.cs (prior to .NET6).

    ```csharp
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
    }).AddUWShibboleth();
    ```

## Using Apache on Linux

Headers must be forwarded from the Apache Reverse Proxy into the ASP.NET app running on Kestrel.  This is done using the `RequestHeader` declaration.  You must manually define every header from Shibboleth that you wish to use in the ASP.NET app.   **ShibSessionIndex** is **required** at a minimum, as that is what the library uses to determine if a Shibboleth session is in place.

		RequestHeader set isMemberOf %{isMemberOf}e
		RequestHeader set eppn %{eppn}e
		RequestHeader set sn %{sn}e
		RequestHeader set givenName %{givenName}e
		RequestHeader set mail %{mail}e
		RequestHeader set uid %{uid}e
		RequestHeader set wiscEduPVI %{wiscEduPVI}e
		RequestHeader set ShibSessionIndex %{ShibSessionIndex}e

# Shibboleth Attributes
The library comes pre-loaded with all attribute names utilized in the UW-Madison Shibboleth implementation.  This list can be [viewed here](../../tree/rel/main/src/UW.Shibboleth/ShibbolethAttributeCollection.cs#L22).

By default, the following Shibboleth attributes are added as `Claim`s to the `HttpContext.User.Identity`.
| Shibboleth Attribute | Claim Type                                                                      | UWShibbolethClaimsType | Notes                                                                                |
|----------------------|---------------------------------------------------------------------------------|------------------------|--------------------------------------------------------------------------------------|
| givenName            | http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname                 | FIRSTNAME              | Not a default attribute, must be released by DoIT IAM per app                        |
| sn                   | http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname                   | LASTNAME               | Not a default attribute, must be released by DoIT IAM per app                        |
| wiscEduPVI           | http://schemas.xmlsoap.org/ws/2005/05/identity/claims/privatepersonalidentifier | PVI                    |                                                                                      |
| uid                  | http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name                      | UID                    | Changed to lowercase                                                                 |
| eppn                 | http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn                       | EPPN                   |                                                                                      |
| mail                 | http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress              | MAIL                   | Changed to lowercase.  Not a default attribute, must be released by DoIT IAM per app |
| isMemberOf           | http://schemas.xmlsoap.org/claims/Group                                         | Group                  | Split by semicolon (;) into individual claims for each group                         |

Additional Shibboleth attributes can be added to the processing.   

***Please note: Any additional attributes must be explicitly released per-app by DoIT IAM***


## Customizing Available Attributes

If additional attributes are needed or other organizations wish to use this library and completely replace the default list of attributes, this can be done using the `configureOptions`.

**Add Additional Attributes**
```csharp
// add additional attributes
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
}).AddUWShibboleth(options => {
    options.ShibbolethAttributes.Add("someCustomAttribute");
    options.ShibbolethAttributes.Add("anotherDirectoryAttribute");
});
```

**Replace Attribute List**
```csharp
// replace the entire attribute list with a new list
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
}).AddUWShibboleth(options => {
    options.ShibbolethAttributes = new ShibbolethAttributeCollection(new List<string> {
        "someCustomAttribute",
        "anotherDirectoryAttribute"});
});
```
## Claim Types
Shibboleth attributes are mapped to Claims using one of the following: `ClaimActions.MapAttribute`, `ClaimActions.MapCustomAttribute`, and `ClaimActions.MapCustomMultiValueAttribute` within the `ShibbolethOptions` class.   Examples:
```csharp
//authentication with Shibboleth
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
    // define the options Action<> to add additional attribute to claim mappings
}).AddUWShibboleth(options =>
{
    // The examples below are to illustrate the use of ClaimActions - these specific examples do not need 
    // to be implemented, as they are already done so by the default configuration.

    // straight value-to-claim mapping
    options.ClaimActions.MapAttribute(UWShibbolethClaimsType.FIRSTNAME, "givenName");
    options.ClaimActions.MapAttribute("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "sn");

    // value-to-claim mapping that requires additional processing
    options.ClaimActions.MapCustomAttribute(UWShibbolethClaimsType.UID, "uid", value =>
    {
        return value.ToLower();
    });

    // single value will be parsed to multiple claim values
    options.ClaimActions.MapCustomMultiValueAttribute(UWShibbolethClaimsType.Group, "isMemberOf", value =>
    {
        return value.Split(';').ToList();
    });

});

```
------------

# Development
***Please note: The `AddDevAuthentication()` method has been deprecated as of v7.0 of this library.***

One of the challenges working with Shibboleth is how to mock the authentication during development.  This library provides a way to hook into `OnSelectingProcessor` event in order to "fake" the user data that would normally be provided via Shibboleth headers/server variables.  The manually supplied user data is then extracted and used as if it was taken from a typical Shibboleth session.

```csharp
//authentication with Shibboleth
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
}).AddUWShibboleth(options =>
{
    // only hook into the events during development
    if (builder.Environment.IsDevelopment())
    {
        // creating a new ShibbolethEvents object to send with options
        options.Events = new ShibbolethEvents
        {
            // hook into the OnSelectingProcessor event
            // this event happens before the Shibboleth processor is chosen.
            // we will be supplying our own
            OnSelectingProcessor = ctx =>
            {
                // supply the ShibbolethDevelopmentProcessor which fakes a Shibboleth session
                // provide all Shibboleth attributes and their values
                ctx.Processor = new ShibbolethDevelopmentProcessor(new                   ShibbolethAttributeValueCollection() {
                        new ShibbolethAttributeValue("uid", "bbadger"),
                        new ShibbolethAttributeValue("givenName", "Bucky"),
                        new ShibbolethAttributeValue("sn", "Badger"),
                        new ShibbolethAttributeValue("mail", "bucky.badger@wisc.edu"),
                        new ShibbolethAttributeValue("wiscEduPVI", "UW999A999"),
                        new ShibbolethAttributeValue("isMemberOf", "uw:domain:dept.wisc.edu:administrativestaff;uw:domain:dept.wisc.edu:it:sysadmin")
                    }
                );

                // this is an asynchronous event, so a task must be return unless an 
                // await is used
                return Task.CompletedTask;
            }
        };
    }
});
```


 

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
 - **SampleMVCCore** .Net 6.0 app using the `IAuthenticationBuilder` extensions for both local development and Shibboleth production
