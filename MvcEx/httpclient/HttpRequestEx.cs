using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace MvcEx.httpclient
{
    public class HttpRequestEx : IHttpRequestEx
    {
        private HttpRequest _request;
        private NameValueCollectionEx _form;
        public NameValueCollectionEx Form
        {
            get { return _form; }
        }

        public string Path
        {
            get { return _request.Path; }
        }

        public NameValueCollection Headers
        {
            get { return _request.Headers; }
        }

        public string Method
        {
            get { return this._request.RequestType; }
        }

        public Stream InputStream
        {
            get { return _request.InputStream; }
        }

        public string ContentType
        {
            get { return this._request.ContentType; }
        }

        public HttpRequestEx(HttpRequest request)
        {
            _request = request;
            _form = new NameValueCollectionEx(_request.Form);
        }

        public string MapPath(string virtualPath)
        {
            return _request.MapPath(virtualPath);
        }

        public string RawUrl
        {
            get { return _request.RawUrl; }
        }

        #region IHttpRequestEx Members

        public object GetCookie(string p)
        {
            return _request.Cookies.Get(p);
        }

        #endregion



        public long ContentLength
        {
            get { return _request.ContentLength; }
        }

        public NameValueCollection QueryString
        {
            get { return _request.QueryString; }
        }

        

       
    }
}
