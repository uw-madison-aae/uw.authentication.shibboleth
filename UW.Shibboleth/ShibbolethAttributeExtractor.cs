using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Shibboleth
{
    /// <summary>
    /// Extracts UW Shibboleth attributes from a collection of attributes in a Shibboleth session
    /// </summary>
    public static class ShibbolethAttributeExtractor
    {
        /// <summary>
        /// Extracts Shibboleth attributes from a Shibboleth session collection of headers/variables
        /// </summary>
        /// <param name="sessionCollection">A collection of headers/variables received in a Shibboleth session</param>
        /// <param name="attributes">A list of <see cref="IShibbolethAttribute"/> that is being extracted from the sessionCollection</param>
        /// <returns>An <see cref="IDictionary{String,String}"/> for attributes and values</returns>
        public static ShibbolethAttributeValueCollection ExtractAttributes(IDictionary<string, string> sessionCollection, IEnumerable<IShibbolethAttribute> attributes)
        {
            var ret_dict = new ShibbolethAttributeValueCollection();
            var distinct_ids = attributes.GroupBy(a => a.Id).Select(a => a.First());
            foreach(var attrib in distinct_ids)
            {
                if (sessionCollection.ContainsKey(attrib.Id))
                {
                    ret_dict.Add(new ShibbolethAttributeValue(attrib.Id, sessionCollection[attrib.Id]));
                }
            }

            return ret_dict;
        }

        /// <summary>
        /// Extracts Shibboleth attributes from a Shibboleth session collection of headers/variables
        /// </summary>
        /// <param name="sessionCollection">A collection of headers/variables received in a Shibboleth session</param>
        /// <param name="attributes">A list of <see cref="IShibbolethAttribute"/> that is being extracted from the sessionCollection</param>
        /// <returns>An <see cref="IDictionary{String,String}"/> for attributes and values</returns>
        public static ShibbolethAttributeValueCollection ExtractAttributes(NameValueCollection sessionCollection, IEnumerable<IShibbolethAttribute> attributes)
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
    }
}
