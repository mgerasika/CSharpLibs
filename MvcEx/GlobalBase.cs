using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Text;

namespace MvcEx
{
    public abstract class GlobalBase : System.Web.HttpApplication
    {
        protected virtual void Application_Start(object sender, EventArgs e)
        {
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {
            Debug.Assert(null == this.Session["DataManager"]);
            this.Session["DataManager"] = Factory.Inst().GetAbstactFactory().CreateDataManager();
        }

        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
        }

        protected virtual void Session_End(object sender, EventArgs e)
        {
            Debug.Assert(null != this.Session["DataManager"]);
            this.Session["DataManager"] = null;
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
        }
    }
}
