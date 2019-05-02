using System;
using System.Collections;
using System.Collections.Generic;

namespace UW.Shibboleth
{
    /// <summary>
    /// Collection that stores Shibboleth attribute value by their Id
    /// </summary>
    public class ShibbolethAttributeValueCollection : Dictionary<string, string>
    {

        public ShibbolethAttributeValueCollection()
        {
        }

        public ShibbolethAttributeValueCollection(ShibbolethAttributeValueCollection existing_collection) : base(existing_collection)
        {

        }

        public ShibbolethAttributeValueCollection(IDictionary<string, string> collection) : base(collection)
        {
        }

        public void Add(ShibbolethAttributeValue attributeValue)
        {
            Add(attributeValue.Id, attributeValue.Value);
        }

        public new ShibbolethAttributeValue this[string attributeId]
        {
            get
            {
                return new ShibbolethAttributeValue(attributeId, base[attributeId]);
            }
        }

        public KeyCollection Ids
        {
            get
            {
                return Keys;
            }
        }

        public bool ContainsId(string key)
        {
            return ContainsKey(key);
        }

        public bool ValueIsNullOrEmpty(string key)
        {
            return string.IsNullOrEmpty(this[key].Value);
        }
    }


}
