using System.Text;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.commet
{
    public class HttpCommetHandler : HttpHandlerBase
    {
        public override void Process(IHttpContextEx httpContext)
        {
            BeginRequest(httpContext);
        }

        public void BeginRequest(IHttpContextEx httpContext)
        {
            HttpCommetContext lContext = new HttpCommetContext(this);
            lContext.HttpContext = httpContext;
            HttpCommetManager.Inst().AddCommetContext(lContext);
            /*
            ObjectEx  lData = HttpCommetManager.Inst().GetDataItem();
            if (null != lData)
            {
                lContext.CompleteRequest(lData);
            }
            else
            {
                HttpCommetManager.Inst().AddCommetContext(lContext);
            }*/
        }

        public virtual void CompleteRequest(IHttpContextEx httpContext, byte[] responseBytes)
        {
            SendResponse(httpContext, responseBytes);
        }

        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            httpResponse.AppendHeader("Access-Control-Allow-Origin","*");
            base.AddHeaders(httpResponse);
            httpResponse.AppendHeader("Content-Type", "text/plain");
        }

    }
}
