using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Text;
using HttpServer.commet;
using HttpServer.websocket;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
{
    public abstract class HttpServerBase
    {
        private class HttpHandlerItem
        {
            public string Ext { get; set; }
            public HttpHandlerBase Handler { get; set; }
        }

        public abstract void Start();
        public abstract void Stop();

        
        private List<HttpHandlerItem> _httpHandlers = new List<HttpHandlerItem>();

        public HttpServerBase() {
            this.DefaultDocument = "index.htm";
        }

        internal static string GetUrlFromConfig()
        {
            string host = ConfigurationManager.AppSettings["httpServerIp"];
            Debug.Assert(!string.IsNullOrEmpty(host));
            string port = ConfigurationManager.AppSettings["httpServerPort"];
            Debug.Assert(!string.IsNullOrEmpty(port));
            string url = String.Format("http://{0}:{1}/", host, port);
            return url;
        }

        public void RegisterHandler(string ext,HttpHandlerBase handler)
        {
            _httpHandlers.Add(new HttpHandlerItem() {Ext = ext,Handler = handler});
        }

        protected virtual HttpHandlerBase CreateHttpHandler(IHttpContextEx lHttpContext)
        {
            HttpHandlerBase lRes = null;

            string httpMethod = lHttpContext.Request.Method;
            string lUrl = lHttpContext.Request.Path;
            string rawUrl = lHttpContext.Request.RawUrl;
            string contentType = lHttpContext.Request.ContentType;
            
            do
            {
                foreach (HttpHandlerItem lItem in _httpHandlers)
                {
                    if (rawUrl.Contains(lItem.Ext))
                    {
                        lRes = Activator.CreateInstance(lItem.Handler.GetType()) as HttpHandlerBase;
                        break;
                    }
                }
                if (null != lRes)
                {
                    break;
                }
                if ("OPTIONS" != httpMethod && lUrl.ToLower().Contains("commet") && rawUrl.Contains("jsonp.js"))
                {
                    lRes = new HttpJsonpCommetHandler();
                    break;
                }
                if (rawUrl.Contains("jsonp.js"))
                {
                    lRes = new HttpJsonpHandler();
                    break;
                }

                string upgradeHeader = lHttpContext.Request.Headers["Upgrade"] ?? "";
                string connectionHeader = lHttpContext.Request.Headers["Connection"] ?? "";
                if (upgradeHeader.ToLower().Contains("websocket") && connectionHeader.ToLower().Contains("upgrade"))
                {
                    lRes = new AcceptWebSocketHandler();
                    break;
                }
                if ("OPTIONS" != httpMethod && lUrl.Contains(".mvc") && lUrl.ToLower().Contains("commet"))
                {
                    lRes = new HttpCommetHandler();
                    break;
                }
                if (lUrl.Contains(".mvc"))
                {
                    lRes = new HttpMvcHandler();
                    break;
                }
                if (!string.IsNullOrEmpty(contentType) &&
                    (contentType.Contains("application/x-www-form-urlencoded") || contentType.Contains("json")))
                {
                    lRes = new HttpJsonHandler();
                    break;
                }
                if (httpMethod == "GET" && rawUrl.IndexOf('/') == 0 && rawUrl.Length >= 1)
                {
                    if (rawUrl.Equals("/")) {
                        lRes = new HttpHtmlHandler();
                        break;
                    }
                    if (rawUrl.Contains(".js"))
                    {
                        lRes = new HttpScriptHandler();
                        break;
                    }
                    if (rawUrl.Contains(".aspx"))
                    {
                        lRes = new HttpAspxHandler();
                        break;
                    }
                    if (rawUrl.Contains(".css"))
                    {
                        lRes = new HttpCssHandler();
                        break;
                    }
                    if (rawUrl.Contains(".htm") || rawUrl.Contains(".html"))
                    {
                        lRes = new HttpHtmlHandler();
                        break;
                    }
                    if (rawUrl.Contains(".swf"))
                    {
                        lRes = new HttpSwfHandler();
                        break;
                    }
                    if (rawUrl.Contains(".png") || rawUrl.Contains(".gif") || rawUrl.Contains(".jpg") || rawUrl.Contains(".bmp"))
                    {
                        lRes = new HttpImageHandler();
                        break;
                    }
                    if (rawUrl.Contains(".mp3") ||
                        rawUrl.Contains(".wav") ||
                        rawUrl.Contains(".ttf") ||
                        rawUrl.Contains(".mp4")
                        )
                    {
                        lRes = new HttpFileHandler();
                        break;
                    }

                    
                    if (rawUrl.Contains("."))
                    {
                        lRes = new HttpErrorHandler() {Message = "You don't have acess to this file"};
                        break;
                    }

                    lRes = new HttpErrorHandler(){Message = String.Format("Handler not found for url = {0}", rawUrl)};
                }
            } while (false);

            return lRes;
        }

        protected void Process(IHttpContextEx lHttpContext)
        {
            HttpHandlerBase lHandler = CreateHttpHandler(lHttpContext);

            if (null != lHandler)
            {
                try
                {
                    lHandler.Process(lHttpContext);
                }
                catch (Exception ex)
                {
                    lHandler = new HttpErrorHandler();
                    (lHandler as HttpErrorHandler).Message = ex.Message;
                    (lHandler as HttpErrorHandler).InnerMessage = ex.InnerException != null ? ex.InnerException.ToString() : ex.ToString();
                    lHandler.Process(lHttpContext);
                }
            }
        }

        public string DefaultDocument { get; set; }
    }
}
