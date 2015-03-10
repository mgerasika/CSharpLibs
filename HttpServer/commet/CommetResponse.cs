using System;
using MvcEx;

namespace HttpServer.commet
{
    public class CommetResponseBase : ResponseBase
    {
    }

    public class ConfirmCommetResponse : CommetResponseBase
    {
        public string ConfirmID { get; set; }

        public ConfirmCommetResponse()
        {
            this.ConfirmID = (Guid.NewGuid()).ToString();
        }
    }
}
