using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Configuration options for <see cref="ShibbolethHandler"/>.
    /// </summary>
    public class ShibbolethOptions : AuthenticationSchemeOptions
    {
        public ShibbolethOptions()
        {
            ClaimsIssuer = ShibbolethDefaults.Issuer;
            CallbackPath = new PathString("/signin-shibboleth");

            // needed for compatiblity as an External Provider in AddDefaultIdentity
            ClaimActions.MapCustomAttribute(ClaimTypes.NameIdentifier, "uid", value =>
            {
                return value.ToLower();
            });

            ClaimActions.MapAttribute(UWShibbolethClaimsType.FIRSTNAME, "givenName");
            ClaimActions.MapAttribute(UWShibbolethClaimsType.LASTNAME, "sn");
            ClaimActions.MapAttribute(UWShibbolethClaimsType.PVI, "wiscEduPVI");
            ClaimActions.MapAttribute(UWShibbolethClaimsType.EPPN, "eppn");

            ClaimActions.MapCustomAttribute(UWShibbolethClaimsType.UID, "uid", value =>
            {
                return value.ToLower();
            });

            ClaimActions.MapCustomAttribute(UWShibbolethClaimsType.EMAIL, "mail", value =>
            {
                return value.ToLower();
            });

            ClaimActions.MapCustomMultiValueAttribute(UWShibbolethClaimsType.Group, "isMemberOf", value =>
            {
                return value.Split(';').ToList();
            });
        }

        /// <summary>
        /// A collection of Shibboleth claim actions used to select values from the user data and create Claims.
        /// </summary>
        public ShibbolethClaimActionCollection ClaimActions { get; } = new ShibbolethClaimActionCollection();

        /// <summary>
        /// The challenge path within the application's pase bath where the ticket will be populated with Shibboleth session information
        /// </summary>
        /// <value>Local url that is secured with Shibboleth</value>
        [Obsolete("Use CallbackPath instead")]
        public PathString ChallengePath => CallbackPath;

        /// <summary>
        /// The request path within the application's base path where the user-agent will be returned.
        /// The middleware will process this request when it arrives.
        /// </summary>
        public PathString CallbackPath { get; set; }

        /// <summary>
        /// Gets or sets whether a challenge that is initiated should be processed
        /// </summary>
        /// <remarks>If false, will produce a 401 Status Code for challenge</remarks>
        [Obsolete("Deprecated - UseChallenge replaces this field.")]
        public bool ProcessChallenge => UseChallenge;

        /// <summary>
        /// Gets or sets whether a challenge that is initiated should be processed.
        /// If <see langword="false"/>, will produce a 401 Status Code for challenge.
        /// This is typically left to <see langword="false"/> when Shibboleth protects an entire site.
        /// Set to <see langword="true"/> when mimicking an OAuth-style site.
        /// </summary>
        public bool UseChallenge { get; set; } = false;

        /// <summary>
        /// Gets or sets the name of the parameter used to convey the original location
        /// of the user before the challenge was triggered.
        /// </summary>
        // Note: this deliberately matches the default parameter name used by the cookie handler.
        public string ReturnUrlParameter { get; set; } = "ReturnUrl";

        /// <summary>
        /// Gets or sets the <see cref="IShibbolethAttributeCollection"/> that contains all attributes to extract
        /// from the Shibboleth headers/variables
        /// </summary>
        public IShibbolethAttributeCollection ShibbolethAttributes { get; set; } = ShibbolethAttributeCollection.DefaultUWAttributes;

        /// <summary>
        /// Gets or sets the authentication scheme corresponding to the middleware
        /// responsible for persisting user's identity after a successful authentication.
        /// This value typically corresponds to a cookie middleware registered in the Startup class.
        /// It is ignored if <see cref="UseChallenge"/> is <see langword="false"/>.
        /// When omitted, <see cref="AuthenticationOptions.DefaultSignInScheme"/> is used as a fallback value.
        /// </summary>
        public string? SignInScheme { get; set; }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; } = default!;

        /// <summary>
        /// Gets or sets the type used to secure data.
        /// </summary>
        public IDataProtectionProvider? DataProtectionProvider { get; set; }

        /// <summary>
        /// The object provided by the application to process events raised by the Shibboleth handler.
        /// The application may implement the interface fully, or it may create an instance of <see cref="ShibbolethEvents"/>
        /// and assign delegates only to the events it wants to process.
        /// </summary>
        public new ShibbolethEvents Events
        {
            get { return (ShibbolethEvents)base.Events!; }
            set { base.Events = value; }
        }

        /// <summary>
        /// Checks that the options are valid for a specific scheme
        /// </summary>
        /// <param name="scheme">The scheme being validated.</param>
        public override void Validate(string scheme)
        {
            base.Validate(scheme);
            if (UseChallenge && string.Equals(scheme, SignInScheme, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("The SignInScheme for a Shibboleth authentication handler cannot be set to itself.  If it was not explicitly set, the AuthenticationOptions.DefaultSignInScheme or DefaultScheme is used.");
            }
        }
    }
}
