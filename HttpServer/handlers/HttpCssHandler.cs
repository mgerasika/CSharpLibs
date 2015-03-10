using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.handlers
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
