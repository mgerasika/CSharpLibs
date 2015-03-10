using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Web;
using CoreEx;
using HttpServer.httplistener;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.handlers
{
    
    public class HttpJsonpHandler : HttpHandlerBase
    {
        public static void FixContext(IHttpContextEx httpContext)
        {
            string method = httpContext.Request.QueryString["method"].ToUpper();
            string callback = httpContext.Request.QueryString["callback"];
            string controller = httpContext.Request.QueryString["controller"];
            string action = httpContext.Request.QueryString["action"];
            int idx = httpContext.Request.Path.LastIndexOf("/jsonp.js");
            Debug.Assert(idx != -1);
            string url = httpContext.Request.Path.Remove(idx);
            
            IHttpRequestExSetter requestSetter = httpContext.Request as IHttpRequestExSetter;
            Debug.Assert(null!= requestSetter);
            requestSetter.Method = method;
            requestSetter.Path = String.Format("{0}/{1}.mvc/{2}",url,controller,action);

            string args = httpContext.Request.QueryString["args"];
            if (!string.IsNullOrEmpty(args))
            {
                IDictionary dict = Utils.DeserializeStr(args) as IDictionary;
                requestSetter.Form = new NameValueCollectionEx();
                if (null != dict)
                {
                    foreach (string key in dict.Keys)
                    {
                        object val = dict[key];
                        httpContext.Request.Form.Add(key, val);
                    }
                }


                args = HttpUtility.UrlDecode(args);
                byte[] argsBuffer = Utils.DefaultEncoding.GetBytes(args);
                requestSetter.InputStream = new MemoryStream(argsBuffer);
            }
        }


        public static byte[] AddJSONPFn(IHttpContextEx httpContext,byte[] responseBytes)
        {
            string callback = httpContext.Request.QueryString["callback"];
            byte[] start = Utils.DefaultEncoding.GetBytes("js.callbackHelper.inst().callFn('" + callback + "',");
            byte[] end = Utils.DefaultEncoding.GetBytes(");");
            byte[] buffer = new byte[start.Length + responseBytes.Length + end.Length];
            start.CopyTo(buffer, 0);
            responseBytes.CopyTo(buffer, start.Length);
            end.CopyTo(buffer, start.Length + responseBytes.Length);
            return buffer;
        }
        public override void Process(IHttpContextEx httpContext)
        {
            FixContext(httpContext);

            string controller = httpContext.Request.QueryString["controller"];
            string action = httpContext.Request.QueryString["action"];
            byte[] responseBytes = MvcProcessorEx.Inst().InvokeRequest(httpContext, controller, action, "");
            byte[] buffer = HttpJsonpHandler.AddJSONPFn(httpContext,responseBytes);
            SendResponse(httpContext,buffer);
        }

        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);

            httpResponse.ContentType = "text/javascript";
        }


    }
}
