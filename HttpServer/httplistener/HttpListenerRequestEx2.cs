using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using MvcEx;

namespace HttpServer.httplistener
{
    public class HttpListenerRequestEx2 : IHttpRequestEx, IHttpRequestExSetter
    {
        private HttpListenerRequest _request;
        private string _contentType;
       
        public NameValueCollectionEx Form { get; set; }

        public string Path { get; set; }

        public NameValueCollection Headers
        {
            get { return _request.Headers; }
        }

        public string Method { get; set; }

        public Stream InputStream { get; set; }

        public string ContentType
        {
            get { return this._contentType; }
        }

        public HttpListenerRequestEx2(HttpListenerRequest request)
        {
            _request = request;
            this.Path = _request.Url.ToString();
            this.Method = this._request.HttpMethod;
            this.InputStream = _request.InputStream;
            
            _contentType = request.ContentType;
            if(string.IsNullOrEmpty(this.ContentType))
            {
                string xRequestedWidth = request.Headers["X-Requested-With"];
                if (!string.IsNullOrEmpty(xRequestedWidth) && xRequestedWidth.Contains("XMLHttpRequest"))
                {
                    _contentType = "application/json";
                }
            }

        }

        public string MapPath(string virtualPath)
        {
            Debug.Assert(false);
            return null;
            //return _request.MapPath(virtualPath);
        }

        public string RawUrl
        {
            get { return _request.RawUrl; }
        }

        #region IHttpRequestEx Members


        public object GetCookie(string p)
        {
            return _request.Cookies[p];
        }

        #endregion

        #region IHttpRequestEx Members


        public long ContentLength
        {
            get { return _request.ContentLength64; }
        }

        #endregion


        public NameValueCollection QueryString
        {
            get { 
                return _request.QueryString; }
        }
    }
}