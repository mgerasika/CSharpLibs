using System;
using System.Diagnostics;
using MvcEx;

namespace HttpServer.commet
{
    public class ServiceCommetBaseController : ServiceBaseController
    {
        public override JsonResult Process(RequestBase request)
        {
            RequestBase lRequest = this.HttpContext.GetFromSession("TempData") as RequestBase;
            if (null != lRequest)
            {
                request = lRequest;
            }
            else
            {
                ResponseBase lResponse = this.HttpContext.GetFromSession("TempData") as ResponseBase;
                if (null != lResponse)
                {
                    request = new CommetRequestBase();
                    (request as CommetRequestBase).Response = lResponse;
                }
            }
            return base.Process(request);
        }

        public JsonResult Process()
        {
            RequestBase request = null;
            RequestBase lRequest = this.HttpContext.GetFromSession("TempData") as RequestBase;
            if (null != lRequest)
            {
                request = lRequest;
            }
            else
            {
                ResponseBase lResponse = this.HttpContext.GetFromSession("TempData") as ResponseBase;
                if (null != lResponse)
                {
                    request = new CommetRequestBase();
                    (request as CommetRequestBase).Response = lResponse;
                }
            }
            Debug.Assert(null != request);

            return base.Process(request);
        }
    }
} 
    