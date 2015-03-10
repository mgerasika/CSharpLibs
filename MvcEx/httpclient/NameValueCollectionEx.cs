using System.Collections.Generic;
using System.Collections.Specialized;

namespace MvcEx.httpclient
{
    public class NameValueCollectionEx
    {
        private Dictionary<string,object> _dictionary = new Dictionary<string, object>(); 
        public NameValueCollectionEx(NameValueCollection items)
        {
            foreach (string s in items.Keys)
            {
                  Add(s,items[s]);
            }
        }

        public NameValueCollectionEx()
        {
        }

        internal object Get(string p)
        {
            if (_dictionary.ContainsKey(p))
            {
                return _dictionary[p];
            }
            return null;
        }

        public void Add(string key, object val)
        {
            _dictionary[key] = val;
        }
    }
}
