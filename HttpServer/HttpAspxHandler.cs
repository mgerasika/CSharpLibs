using System;
using System.Collections.Generic;
using System.IO;

using System.Text;
using System.Web;
using System.Web.UI;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
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
