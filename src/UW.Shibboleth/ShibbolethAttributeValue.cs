using System.Collections.Generic;

namespace UW.Shibboleth
{
    /// <summary>
    /// Value object that stores a Shibboleth attribute and value
    /// </summary>
    public class ShibbolethAttributeValue 
    {
        private KeyValuePair<string, string> storage;
        public ShibbolethAttributeValue(string id, string value)
        {
            storage = new KeyValuePair<string, string>(id, value);
        }

        public string Id { get { return storage.Key; } }
        public string Value { get { return storage.Value; } }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Value))
                return Value;
            else
                return base.ToString();
        }
    }
}
