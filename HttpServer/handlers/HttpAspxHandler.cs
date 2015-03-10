using System.Web;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.handlers
{
    class HttpAspxHandler : HttpHandlerBase
    {
        public override void Process(IHttpContextEx httpContext)
        {
            HttpApplication app = new HttpApplication();
            HttpServerUtility server = app.Context.Server;
            base.Process(httpContext);
        }
    }
}
