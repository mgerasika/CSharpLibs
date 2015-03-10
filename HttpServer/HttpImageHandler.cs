using System;
using System.Collections.Generic;

using System.Text;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
{
    class HttpImageHandler : HttpFileHandler
    {
        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);
        }
    }
}
