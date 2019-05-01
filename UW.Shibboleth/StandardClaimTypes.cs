using System.Security.Claims;

namespace UW.Identity
{
    /// <summary>
    /// Standardized claim types for use across varying authentication platforms
    /// </summary>
    public class StandardClaimTypes
    {
        public const string Group = "http://schemas.xmlsoap.org/claims/Group";

        /// <summary>
        /// Private personal identifier
        /// </summary>
        public const string PPID = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/privatepersonalidentifier";

        /// <summary>
        /// User principal name of the user
        /// </summary>
        public const string UPN = ClaimTypes.Upn;

        
        /// <summary>
        /// Unique name of user
        /// </summary>
        public const string Name = ClaimTypes.Name;

        public const string GivenName = ClaimTypes.GivenName;
        public const string Surname = ClaimTypes.Surname;
        public const string Email = ClaimTypes.Email;
    }
}
