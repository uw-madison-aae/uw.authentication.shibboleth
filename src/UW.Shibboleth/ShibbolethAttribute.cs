using System.Xml.Serialization;

namespace UW.Shibboleth
{
    /// <summary>
    /// concrete implementation of <see cref="IShibbolethAttribute"/>
    /// </summary>
    [XmlRoot("Attribute")]
    public class ShibbolethAttribute : IShibbolethAttribute
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
