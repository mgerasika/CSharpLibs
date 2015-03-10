using System.IO;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.httplistener
{
    public interface IHttpRequestExSetter
    {
        string Path { set; }
        string Method { set; }
        Stream InputStream { set; }
        NameValueCollectionEx Form { set; }
    }
}
