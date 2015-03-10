using System;
using System.Collections.Generic;

using System.Text;

namespace MvcEx
{
    public class ServiceEx : IDisposable
    {
        public ResponseBase Process(RequestBase request)
        {
            ResponseBase lResponse = null;
            try
            {
                request.Validate();
                lResponse = request.Send();
            }
            catch(Exception ex)
            {
                lResponse = new ErrorResponse() {Message = ex.ToString()};
            }
            return lResponse;
        }


        public void Dispose()
        {
        }
    }
}
