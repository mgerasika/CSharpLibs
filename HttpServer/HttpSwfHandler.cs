using System;
using System.Collections.Generic;

using System.Net;
using System.Text;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
{
    class HttpSwfHandler : HttpFileHandler
    {
        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);

            httpResponse.ContentType = "application/octet-stream";
        }
    }
}
