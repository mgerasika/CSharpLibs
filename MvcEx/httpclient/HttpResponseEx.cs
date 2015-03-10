using System.Web;

namespace MvcEx.httpclient
{
    public class HttpResponseEx : IHttpResponseEx
    {
        private HttpResponse _response;

        public HttpResponseEx(HttpResponse response)
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

        public int ContentLength { get; set; }

        public void WriteToOutputStream(byte[] buffer)
        {
            _response.BinaryWrite(buffer);
        }

        public void AppendHeader(string key, string val)
        {
            _response.AppendHeader(key,val);
        }

        public void CloseStream()
        {
        }

        public void AddCookie(object cookieSession)
        {
            _response.Cookies.Add(cookieSession as HttpCookie);
        }
    }//HttpResponseEx
}
