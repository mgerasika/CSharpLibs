using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using System.Net;
using System.Text;
using CoreEx;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
{
    public class HttpJsonHandler : HttpHandlerBase
    {
        public override void Process(IHttpContextEx httpContext)
        {
            IHttpRequestEx httpRequest = httpContext.Request;

            string requestData = "";
            using (StreamReader sr = new StreamReader(httpRequest.InputStream))
            {
                requestData = sr.ReadToEnd();
            }
            Debug.Assert(!string.IsNullOrEmpty(requestData));

            object requestObject = Utils.DeserializeStr(requestData);
            RequestBase requestBase = Utils.DeserializeObject(requestObject) as RequestBase;
            Debug.Assert(null != requestBase);
            //Debug.Assert(Guid.Empty != requestBase.DebugID);

            ResponseBase responseBase = null;
            try
            {
                responseBase = requestBase.Send();
                responseBase.DebugID = requestBase.DebugID;
            }
            catch (Exception ex)
            {
                Logger.Inst.Error("Request failed " + ex.ToString());
                responseBase = new ErrorResponse() { Message = ex.ToString() };
                responseBase.DebugID = requestBase.DebugID;
            }

            Debug.Assert(null != responseBase);
            byte[] responseBytes = Utils.Serialize(responseBase);
            SendResponse(httpContext, responseBytes);
        }
    }
}
