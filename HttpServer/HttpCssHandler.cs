using System;
using System.Collections.Generic;

using System.Net;
using System.Text;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
{
    class HttpCssHandler : HttpFileHandler
    {
        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);

            httpResponse.ContentType = "text/css;charset=UTF-8";
        }
    }
}
