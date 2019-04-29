using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace UW.Shibboleth.Xml
{
    [XmlRoot(ElementName ="Attributes", Namespace = "urn:mace:shibboleth:2.0:attribute-map", IsNullable = false)]
    public class AttributesRoot 
    {
        [XmlElement("Attribute")]
        public ShibbolethAttribute[] Attribute { get; set; }


    }
}
