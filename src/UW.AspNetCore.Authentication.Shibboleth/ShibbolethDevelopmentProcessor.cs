using Microsoft.AspNetCore.Http;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication
{
    /// <summary>
    /// Used to process supplied values as if they were retrieved from a Shibboleth session
    /// </summary>
    public class ShibbolethDevelopmentProcessor : IShibbolethProcessor
    {
        public ShibbolethAttributeValueCollection Attributes { get; }
        public bool IsSession { get; }

        public ShibbolethDevelopmentProcessor(ShibbolethAttributeValueCollection attributes, bool isSession = true)
        {
            Attributes = attributes;
            IsSession = isSession;
        }
        public ShibbolethAttributeValueCollection ExtractAttributeValues(HttpContext context)
        {
            return Attributes;
        }

        public bool IsShibbolethSession(HttpContext context) => IsSession;
    }
}
