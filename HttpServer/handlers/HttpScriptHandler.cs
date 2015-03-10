using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.handlers
{
    public class HttpScriptHandler : HttpFileHandler
    {
        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);

            httpResponse.ContentType = "text/javascript";
        }
    }
}
