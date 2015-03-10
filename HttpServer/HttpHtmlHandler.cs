using System;
using System.Collections.Generic;

using System.Text;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
{
    class HttpHtmlHandler : HttpFileHandler
    {
        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);

            httpResponse.ContentType = "text/html; charset=UTF-8";
        }
    }
}
