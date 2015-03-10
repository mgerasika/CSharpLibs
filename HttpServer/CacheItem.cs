using System;
using System.Collections.Generic;

using System.Text;

namespace HttpServer
{
#if !DEBUG
    public class CacheItem
    {
        public string Url { get; set; }
        public byte[] Content { get; set; }
    }
#endif
}
