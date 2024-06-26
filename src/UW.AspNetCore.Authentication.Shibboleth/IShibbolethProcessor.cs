using Microsoft.AspNetCore.Http;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication;

public interface IShibbolethProcessor
{
    /// <summary>
    /// Extracts Shibboleth attributes from a Shibboleth session collection of headers/variables
    /// </summary>
    /// <returns>An <see cref="IDictionary{String,String}"/> for attributes and values</returns>
    ShibbolethAttributeValueCollection ExtractAttributeValues(HttpContext context);

    /// <summary>
    /// Returns whether there is a Shibboleth session present (a user is authenticated)
    /// </summary>
    bool IsShibbolethSession(HttpContext context);
}
