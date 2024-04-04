using Microsoft.AspNetCore.Http;
using UW.Shibboleth;

namespace UW.AspNetCore.Authentication;

/// <summary>
/// Processes Shibboleth authentication in Variable mode
/// </summary>
public class ShibbolethVariableProcessor : IShibbolethProcessor
{
    /// <summary>
    /// Shibboleth attribute ids from the IDP
    /// </summary>
    protected IShibbolethAttributeCollection Attributes { get; }

    public ShibbolethVariableProcessor(IShibbolethAttributeCollection attributes)
    {
        Attributes = attributes;
    }

    public bool IsShibbolethSession(HttpContext context)
    {
        // look for the presence of the Shib-Session-Index - indicates a Shibboleth session in effect
        return !string.IsNullOrEmpty(context.GetServerVariable(ShibbolethDefaults.VariableShibIndexName));
    }

    /// <summary>
    /// Extracts Shibboleth attributes from a Shibboleth session context of server variables
    /// </summary>
    /// <param name="context"></param>
    /// <returns>An <see cref="IDictionary{String,String}"/> for attributes and values</returns>
    public ShibbolethAttributeValueCollection ExtractAttributeValues(HttpContext context)
    {
        var attributeValues = new ShibbolethAttributeValueCollection();

        foreach (string attribute in Attributes)
        {
            string value = context.GetServerVariable(attribute);
            if (!string.IsNullOrEmpty(value))
            {
                attributeValues.Add(new ShibbolethAttributeValue(attribute, value));
            }
        }

        return attributeValues;
    }
}
