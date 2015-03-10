using System;
using System.Collections.Generic;
using System.IO;

using System.Net;
using System.Text;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
{
    public class HttpHandlerBase
    {
        public IHttpContextEx HttpContext { get; set; }

        public virtual void Process(IHttpContextEx httpContext)
        {
            this.HttpContext = httpContext;
        }

        protected virtual void AddHeaders(IHttpResponseEx httpResponse)
        {
            httpResponse.AppendHeader("Server", "DarwinsGrove");
        }

        protected virtual void SendResponse(IHttpContextEx httpContext, byte[] responseBytes)
        {
            IHttpResponseEx httpResponse = httpContext.Response;
            httpResponse.ContentLength = responseBytes.Length;
            AddHeaders(httpResponse);
            httpResponse.WriteToOutputStream(responseBytes);
        }
    }
}
