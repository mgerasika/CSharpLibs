using System;
using System.Collections.Generic;

using System.Text;

namespace HttpServer
{
#if !DEBUG
    public class CacheManager
    {
        private static object _lockObject = new object();
        private static CacheManager _instance;
        private Dictionary<string,CacheItem> _chacheDictionary = new Dictionary<string, CacheItem>();

        public bool HasItem(string url)
        {
            return _chacheDictionary.ContainsKey(url);
        }

        public CacheItem GetItem(string url)
        {
            return _chacheDictionary[url];
        }

        public void AddItem(CacheItem item)
        {
            lock (_chacheDictionary)
            {
                if (!_chacheDictionary.ContainsKey(item.Url))
                {
                    _chacheDictionary.Add(item.Url, item);
                }
            }
        }

        private CacheManager()
        {
            
        }

        public static CacheManager Inst()
        {
            if(null == _instance)
            {
                _instance = new CacheManager();
            }
            return _instance;
        }
    }
#endif
}
