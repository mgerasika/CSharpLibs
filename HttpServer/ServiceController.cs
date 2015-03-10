using System;
using MvcEx;

namespace HttpServer
{
    public class ServiceBaseController : ControllerBase
    {
        public virtual JsonResult Process(RequestBase request)
        {
            ResponseBase response = ProcessRequest(request);
            return Json(response);
        }

        public virtual ResponseBase ProcessRequest(RequestBase request)
        {
            request.Validate();
            ResponseBase response = null;
            try
            {
                response = request.Send();
            }
            catch (Exception ex)
            {
                response = new ErrorResponse() { Message = ex.ToString() };
            }
            return response;
        }
    }
} 
    