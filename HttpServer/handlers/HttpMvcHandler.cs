using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.handlers
{
    public class HttpMvcHandler : HttpHandlerBase
    {
        public override void Process(IHttpContextEx httpContext)
        {
            MvcProcessorEx.Inst().ProcessRequest(httpContext);
        }
    }
}
