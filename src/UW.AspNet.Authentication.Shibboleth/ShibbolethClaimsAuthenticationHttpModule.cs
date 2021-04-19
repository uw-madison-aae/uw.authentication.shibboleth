using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using UW.Shibboleth;

namespace UW.AspNet.Authentication
{
    /// <summary>
    /// Base module for processing a request containing a Shbboleth user and populating a ClaimsPrincipal
    /// </summary>
    public class ShibbolethClaimsAuthenticationHttpModule : ClaimsAuthenticationHttpModule
    {
        public override IPrincipal GetClaimsPrincipal(HttpContext context)
        {


            // check if this is even a Shibboleth session
            var shibSessionType = IsShibbolethSession(context.Request);

            if (shibSessionType != ShibbolethSessionType.None)
            {
                var attributes = GetAttributesFromRequest(context.Request, shibSessionType);

                return CreateClaimsPrincipal(attributes);
            } else
            {
                return context.User;
            }

        }

        /// <summary>
        /// Retrieves Shibboleth attributes from the Shibboleth session collection
        /// </summary>
        protected virtual ShibbolethAttributeValueCollection GetAttributesFromRequest(HttpRequest request, ShibbolethSessionType sessionType) {

            var attributes = GetShibbolethAttributes();
            switch (sessionType)
            {
                case ShibbolethSessionType.Header:
                    return ExtractAttributes(request.Headers, attributes);
                case ShibbolethSessionType.Variable:
                    return ExtractAttributes(request.ServerVariables, attributes);
                default:
                    throw new System.Exception($"Invalid Shibboleth session type - cannot extract attributes for {sessionType}");
            }
        
        }

        /// <summary>
        /// Whether the session collection indicates this is a Shibboleth session
        /// </summary>
        protected virtual ShibbolethSessionType IsShibbolethSession(HttpRequest request)
        {
            // check for variables first.  This is the preferred method when using IIS7 and considered more secure
            // https://wiki.shibboleth.net/confluence/display/SP3/AttributeAccess#AttributeAccess-ServerVariables

            // must be done with the NameValueCollection because Shibboleth Server Variables don't show up in the AllKeys
            if (request.ServerVariables.GetValues(ShibbolethAuthenticationDefaults.VariableShibIndexName) != null)
                return ShibbolethSessionType.Variable;

            // nothing found in the server variables.  Now check for a session in the headers
            if (request.Headers.GetValues(ShibbolethAuthenticationDefaults.HeaderShibIndexName) != null)
                return ShibbolethSessionType.Header;

            // nothing found - no Shibboleth session
            return ShibbolethSessionType.None;

        }


        /// <summary>
        /// Returns the attribute mapping from the XML stored in UW.Shibboleth
        /// </summary>
        public virtual IList<IShibbolethAttribute> GetShibbolethAttributes()
        {
            return ShibbolethDefaultAttributes.GetAttributeMapping();

        }

        /// <summary>
        /// Creates a <see cref="ClaimsIdentity"/> using the minimum Shibboleth attributes
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        public virtual ClaimsPrincipal CreateClaimsPrincipal(ShibbolethAttributeValueCollection userData)
        {

            var schemeName = ShibbolethAuthenticationDefaults.AuthenticationScheme;
            var claimsIssuer = ShibbolethAuthenticationDefaults.Issuer;

            var identity = new ClaimsIdentity(schemeName);

            var claimActions = GetClaimActions();

            // examine the specified user data, determine if requisite data is present, and optionally add it
            foreach (var action in claimActions)
            {
                action.Run(userData, identity, claimsIssuer ?? schemeName);
            }

            return new ClaimsPrincipal(identity);
        }

        protected virtual ShibbolethClaimActionCollection GetClaimActions()
        {
            var claimActions = new ShibbolethClaimActionCollection();

            claimActions.MapAttribute(UWShibbolethClaimsType.FIRSTNAME, "givenName");
            claimActions.MapAttribute(UWShibbolethClaimsType.LASTNAME, "sn");
            claimActions.MapAttribute(UWShibbolethClaimsType.PVI, "wiscEduPVI");
            claimActions.MapAttribute(UWShibbolethClaimsType.EPPN, "eppn");

            claimActions.MapCustomAttribute(UWShibbolethClaimsType.UID, "uid", value =>
            {
                return value.ToLower();
            });

            claimActions.MapCustomAttribute(UWShibbolethClaimsType.EMAIL, "mail", value =>
            {
                return value.ToLower();
            });

            claimActions.MapCustomMultiValueAttribute(UWShibbolethClaimsType.Group, "isMemberOf", value =>
            {
                return value.Split(';').ToList();
            });

            return claimActions;
        }

        /// <summary>
        /// Extracts Shibboleth attributes from a Shibboleth session collection of headers/variables
        /// </summary>
        /// <param name="sessionCollection">A collection of headers/variables received in a Shibboleth session</param>
        /// <param name="attributes">A list of <see cref="IShibbolethAttribute"/> that is being extracted from the sessionCollection</param>
        /// <returns>An <see cref="IDictionary{String,String}"/> for attributes and values</returns>
        public ShibbolethAttributeValueCollection ExtractAttributes(NameValueCollection sessionCollection, IEnumerable<IShibbolethAttribute> attributes)
        {
            // quirk with ServerVariables for Shibboleth - cannot grabs the keys.  Keys are NOT added to the key collection.  Must request them manually (by supplied the ShibbolethAttribute.Id
            var ret_dict = new ShibbolethAttributeValueCollection();
            var distinct_ids = attributes.GroupBy(a => a.Id).Select(a => a.First());
            foreach (var attrib in distinct_ids)
            {
                if (sessionCollection[attrib.Id] != null)
                {
                    ret_dict.Add(new ShibbolethAttributeValue(attrib.Id, sessionCollection[attrib.Id]));
                }
            }

            return ret_dict;
        }

        protected enum ShibbolethSessionType
        {
            Variable,
            Header,
            None
        }
    }
}