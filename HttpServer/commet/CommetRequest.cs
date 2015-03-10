using System;
using System.Diagnostics;
using MvcEx;

namespace HttpServer.commet
{
    public class CommetRequestBase : RequestBase
    {
        public ResponseBase Response { get; set; }

        public override ResponseBase Send()
        {
            if(null != this.Response)
            {
                return this.Response;
            }
            return new SuccessResponse();
        }
    }

    public class ConfirmCommetRequest : CommetRequestBase
    {
        public string ConfirmID { get; set; }

        public override ResponseBase Send()
        {
            return base.Send();
        }
    }
}
