using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.handlers
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
