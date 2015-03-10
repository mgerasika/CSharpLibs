using System.Net;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.httplistener
{
    public class HttpListenerResponseEx : IHttpResponseEx
    {
        private HttpListenerResponse _response;

        public HttpListenerResponseEx(HttpListenerResponse response)
        {
            _response = response;
        }

        public int StatusCode
        {
            get { return this._response.StatusCode; }
            set { this._response.StatusCode = value; }
        }

        public string ContentType
        {
            get { return this._response.ContentType; }
            set { this._response.ContentType = value; }
        }

        public int ContentLength
        {
            get { return (int)this._response.ContentLength64; }
            set { this._response.ContentLength64 = value; }
        }

        public void WriteToOutputStream(byte[]res)
        {
            _response.OutputStream.Write(res, 0, res.Length);
        }

        public void AppendHeader(string key, string val)
        {
            _response.AppendHeader(key, val);
        }

        public void CloseStream()
        {
            _response.Close();
        }

        public void AddCookie(object cookieSession)
        {
            _response.Cookies.Add(cookieSession as Cookie);
        }
    }
}