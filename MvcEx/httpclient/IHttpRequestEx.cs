using System.Collections.Specialized;
using System.IO;

namespace MvcEx.httpclient
{
    public interface IHttpRequestEx 
    {
        NameValueCollectionEx Form { get; }

        string Path { get; }

        NameValueCollection Headers { get; }

        string Method { get; }

        Stream InputStream { get; }

        string ContentType { get; }

        string MapPath(string virtualPath);

        string RawUrl { get; }
        long ContentLength { get; }
        NameValueCollection QueryString { get; }

        object GetCookie(string p);
    }
}