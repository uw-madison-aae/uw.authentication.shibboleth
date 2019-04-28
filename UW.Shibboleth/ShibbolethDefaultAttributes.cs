using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace UW.Shibboleth
{
    /// <summary>
    /// Creates a list of <see cref="IShibbolethAttribute"/> based on the included attribute-map.xml
    /// </summary>
    public static class ShibbolethDefaultAttributes
    {
        public virtual IList<IShibbolethAttribute> GetAttributeMapping()
        {
            IList<IShibbolethAttribute> attributes = new List<IShibbolethAttribute>();

            // get the default XML file
            var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = System.IO.Path.Combine(buildDir, "attribute-map.xml");

            var xml = XElement.Load(filePath);

            using(XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch(reader.Name.ToString())
                        {
                            case "name":
                                att
                        }
                    }
                }
            }

            var xml = XDocument.Load(filePath);

            var query = from a in xml.Root.Descendants("Attribute")
                        select a.Element

            foreach(string name in query)
            {

            }
        }

        

    }
}
