using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.handlers
{
    class HttpImageHandler : HttpFileHandler
    {
        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);
        }
    }
}
