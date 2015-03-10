using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Web;
using System.Reflection;
using System.Diagnostics;
using System.Web.Configuration;
using System.Web.SessionState;
using MvcEx.httpclient;

namespace MvcEx
{
    public class MvcHttpHandler : IHttpHandler, IRequiresSessionState
    {
        public virtual void ProcessRequest(HttpContext context2)
        {
            IHttpContextEx context = new HttpContextEx(context2);
            MvcProcessorEx.Inst().ProcessRequest(context);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
