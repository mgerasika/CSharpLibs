using CoreEx;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.handlers
{
    class HttpErrorHandler : HttpHandlerBase
    {
        public override void Process(IHttpContextEx httpContext)
        {
            base.Process(httpContext);

            SendResponse(httpContext, Utils.DefaultEncoding.GetBytes(this.Message));
        }

        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);

            httpResponse.StatusCode = 500;
            httpResponse.ContentType = "text/html";
        }

        public string Message { get; set; }

        public string InnerMessage { get; set; }
    }
}
