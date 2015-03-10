using System;
using System.Collections.Generic;

using System.Net;
using System.Text;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
{
    public class HttpMvcHandler : HttpHandlerBase
    {
        public override void Process(IHttpContextEx httpContext)
        {
            MvcProcessorEx.Inst().ProcessRequest(httpContext);
        }
    }
}
