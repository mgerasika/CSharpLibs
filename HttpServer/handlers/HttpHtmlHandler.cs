using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.handlers
{
    class HttpHtmlHandler : HttpFileHandler
    {
        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);

            httpResponse.ContentType = "text/html; charset=UTF-8";
        }

        protected override string GetLocalFileUrl(IHttpContextEx httpContext)
        {
            string lUrl = base.GetLocalFileUrl(httpContext);
            if (string.IsNullOrEmpty(lUrl))
            {
                //default document
                lUrl = "index.htm";
            }
            return lUrl;
        }
    }
}
