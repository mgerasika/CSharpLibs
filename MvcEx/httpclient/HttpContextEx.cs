using System;
using System.Web;

namespace MvcEx.httpclient
{
    public class HttpContextEx : IHttpContextEx
    {
        private HttpContext _context = null;
        public IHttpRequestEx Request { get; set; }
        public IHttpResponseEx Response { get; set; }
        public string SessionID { get; private set; }

        public HttpContextEx(HttpContext context)
        {
            this.Request = new HttpRequestEx(context.Request);
            this.Response = new HttpResponseEx(context.Response);
            this._context = context;
        }

        public void ServerExecute(string virtualPath, System.IO.StringWriter sw)
        {
            _context.Server.Execute(virtualPath,sw);
        }

        public string WebDir
        {
            get { throw new NotImplementedException(); }
        }

        #region IHttpContextEx Members


        public void AddToSession(string key, object value)
        {
            this._context.Session[key] = value;
        }

        public object GetFromSession(string key)
        {
            return  this._context.Session[key];
        }

        public void RemoveFromSession(string key)
        {
            this._context.Session.Remove(key);
        }

        #endregion
    }
}
