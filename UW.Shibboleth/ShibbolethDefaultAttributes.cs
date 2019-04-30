using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UW.Shibboleth.Xml;

namespace UW.Shibboleth
{
    /// <summary>
    /// Creates a list of <see cref="IShibbolethAttribute"/> based on the included attribute-map.xml
    /// </summary>
    public static class ShibbolethDefaultAttributes
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static IList<IShibbolethAttribute> GetAttributeMapping()
        {

            // get the default XML file
            var serializer = new XmlSerializer(typeof(AttributesRoot));

            AttributesRoot attrib_root;

            var resourceName = "UW.Shibboleth.Xml.attribute-map.xml";
            //using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(resourceStream))
                {
                    //sr.ReadToEnd();

                    attrib_root = (AttributesRoot)serializer.Deserialize(sr);
                }

                return new List<IShibbolethAttribute>(attrib_root.Attribute);
            }
        }
    }
}
