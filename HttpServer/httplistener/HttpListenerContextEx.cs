using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.httplistener
{
    public class HttpListenerContextEx : IHttpContextEx
    {
        private HttpListenerContext _context = null;

        public IHttpRequestEx Request { get; set; }
        public IHttpResponseEx Response { get; set; }
        private IDictionary<string,object> Session { get; set; }
        public string SessionID { get; private set; }

        public void AddToSession(string key, object value)
        {
            this.Session[key] = value;
        }

        public void RemoveFromSession(string key)
        {
            this.Session.Remove(key);
        }

        public HttpListenerContextEx(
            HttpListenerContext context,
            IDictionary<string,object> lSession,string sessionId) : this(context)
        {
            this.SessionID = sessionId;
            this.Session = lSession;
        }

        public HttpListenerContextEx(HttpListenerContext context)
        {
            this.Request = new HttpListenerRequestEx(context.Request);
            this.Response = new HttpListenerResponseEx(context.Response);
            this._context = context;
            this.Session = new Dictionary<string, object>();
        }

        public void ServerExecute(string virtualPath, System.IO.StringWriter sw)
        {
            Debug.Assert(false);
            //_context.Server.Execute(virtualPath, sw);
        }

        public string WebDir
        {
            get {
                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                DirectoryInfo projectDir = dir.Parent.Parent;
                DirectoryInfo[] webDirs = projectDir.GetDirectories("web");
                return webDirs[0].FullName;
            }
        }

        #region IHttpContextEx Members


        public object GetFromSession(string key)
        {
            if (this.Session.ContainsKey(key))
            {
                return this.Session[key];
            }
            return null;
        }

        #endregion
    }
}