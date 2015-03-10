using HttpServer.commet;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.handlers
{
    public class HttpJsonpCommetHandler : HttpCommetHandler
    {
        public override void Process(IHttpContextEx httpContext)
        {
            HttpJsonpHandler.FixContext(httpContext);

            base.Process(httpContext);
        }

        public override void CompleteRequest(IHttpContextEx httpContext, byte[] responseBytes)
        {
            byte[] buffer = HttpJsonpHandler.AddJSONPFn(httpContext, responseBytes);
            base.CompleteRequest(httpContext, buffer);
        }
    }
}