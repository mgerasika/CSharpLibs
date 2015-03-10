using System;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Collections.Specialized;
using CoreEx;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.httplistener
{
    public class HttpListenerServer : HttpServerBase
    {
        public event EventHandler<EventArgs> Started;
        public event EventHandler<EventArgs> Stopped;

        private HttpListener _listener;
        private string _url;
     
        public HttpListenerServer(string host,string port) : this(String.Format("http://{0}:{1}/", host, port))
        {
        }

        public HttpListenerServer() : this(GetUrlFromConfig())
        {
        }

        public HttpListenerServer(string url)
        {
            _url = url;
            ThreadPool.SetMinThreads(100, 100);
            ThreadPool.SetMaxThreads(2000, 2000);

            _listener = new HttpListener();
            _listener.Prefixes.Add(_url); 
        }

        public HttpListenerServer(string host, int port) :this(String.Format(@"http://{0}:{1}/",host,port))
        {
           
        }

        public override void Start()
        {
            _listener.Start();
            Logger.Inst.Trace(String.Format("Http server started at {0}",_url));

            if (null != this.Started) {
                this.Started(this,new EventArgs());
            }
            Listen();
        }

        private void Listen()
        {
            while (_listener.IsListening)
            {
                IAsyncResult result = _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        private void ListenerCallback(IAsyncResult lResult)
        {
            if (_listener.IsListening) {
                HttpListenerContext lContext = _listener.EndGetContext(lResult);

                IHttpContextEx lHttpContext = null;
                string lUrl = lContext.Request.Url.ToString();

                do {
                    if (lUrl.Contains(".mvc")) {
                        string lSessionId = String.Empty;
                        NameValueCollection lQueryString = lContext.Request.QueryString;
                        if (null != lQueryString && !string.IsNullOrEmpty(lQueryString.Get("SessionID"))) {
                            lSessionId = lQueryString.Get("SessionID");
                            Debug.Assert(!string.IsNullOrEmpty(lSessionId));
                        }
                        else {
                            //Debug.Assert(false);

                        }
                        SessionItem lSessionItem = SessionManager.Inst().GetSession(lSessionId);
                        lHttpContext = lSessionItem.CreateContext(lContext);
                        break;
                    }

                    lHttpContext = new HttpListenerContextEx(lContext);
                } while (false);

                Process(lHttpContext);
            }
        }

        public override void Stop()
        {
            lock (_listener) {
                _listener.Stop();
                _listener.Abort();
            }

            if (null != this.Stopped)
            {
                this.Stopped(this, new EventArgs());
            }

            Logger.Inst.Trace(String.Format("Server stopped"));
        }

        
    }

}
