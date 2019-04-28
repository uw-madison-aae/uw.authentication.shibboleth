using System;
using System.Collections.Generic;
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
        public static IDictionary<string,string> ExtractAttributes(IDictionary<string, string> sessionCollection, IEnumerable<IShibbolethAttribute> attributes)
        {
            var ret_dict = new Dictionary<string, string>();
            foreach(var attrib in attributes)
            {
                if (sessionCollection.ContainsKey(attrib.Id))
                {
                    ret_dict.Add(attrib.Id, sessionCollection[attrib.Id]);
                }
            }

            return ret_dict;
        }
    }
}
