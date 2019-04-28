using System.Collections.Generic;
using System.Collections.Specialized;

namespace UW.Authentication.AspNet
{
    static class NamedValueCollectionExtensions
    {
        public static IDictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string key in nvc.AllKeys)
            {
                dict.Add(key, nvc[key]);

            }
            return dict;
        }
    }
}
