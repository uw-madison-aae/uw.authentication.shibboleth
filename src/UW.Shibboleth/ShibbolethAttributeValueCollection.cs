using System.Collections.Generic;

namespace UW.Shibboleth
{
    /// <summary>
    /// Collection that stores Shibboleth session attribute values by their Shibboleth Attribute Id
    /// </summary>
    public class ShibbolethAttributeValueCollection : Dictionary<string, ShibbolethAttributeValue>
    {

        public ShibbolethAttributeValueCollection()
        {
        }

        public ShibbolethAttributeValueCollection(ShibbolethAttributeValueCollection existing_collection) : base(existing_collection)
        {

        }

        public ShibbolethAttributeValueCollection(IDictionary<string, ShibbolethAttributeValue> collection) : base(collection)
        {
        }

        public void Add(ShibbolethAttributeValue attributeValue)
        {
            Add(attributeValue.Id, attributeValue);
        }

        public KeyCollection AttributeIds
        {
            get
            {
                return Keys;
            }
        }

        public bool ContainsAttribute(string attributeId)
        {
            return ContainsKey(attributeId);
        }

        public bool ValueIsNullOrEmpty(string attributeId)
        {
            return string.IsNullOrEmpty(this[attributeId].Value);
        }
    }


}
