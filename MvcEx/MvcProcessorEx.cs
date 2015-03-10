using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Reflection;
using System.Text;
using System.Web;
using CoreEx;
using MvcEx.httpclient;

namespace MvcEx
{
	public class MvcProcessorEx
	{
	    private static MvcProcessorEx _instance;

        private MvcProcessorEx()
        {
        }

        public static MvcProcessorEx Inst()
        {
            if(null == _instance)
            {
                _instance = new MvcProcessorEx();
            }
            return _instance;
        }

        public virtual void ProcessRequest(IHttpContextEx lContext)
        {
            string lRes = "";

            try
            {
                Logger.Inst.Trace(lContext.Request.Method + " " + lContext.Request.Path);
                if (lContext.Request.Method == "OPTIONS")
                {
                    lContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                    lContext.Response.AppendHeader("Access-Control-Allow-Headers", lContext.Request.Headers.Get("Access-Control-Request-Headers"));
                }
                else
                {
                    /*ie*/
                    //lContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                    //lContext.Response.AppendHeader("Access-Control-Allow-Headers", lContext.Request.Headers.Get("Access-Control-Request-Headers"));

                    byte[] data = InvokeRequest(lContext);
                    //context.Response.AddHeader("Content-Length", buffer.Length.ToString());
                    lContext.Response.StatusCode = 200;
                    lContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                    lContext.Response.WriteToOutputStream(data);
                   
                    //UtilsEx.Utils.LogTrace(buffer);
                }

                lContext.Response.CloseStream();
            }
            catch (Exception ex)
            {
                lRes = ex.Message;
                //context.Response.AddHeader("Content-Length",buffer.Length.ToString());
                lContext.Response.StatusCode = 500;
                lContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                lContext.Response.WriteToOutputStream(Utils.DefaultEncoding.GetBytes(lRes));

                lContext.Response.CloseStream();
                Logger.Inst.Error(lRes);
            }
        }

        public virtual byte[] InvokeRequest(IHttpContextEx context, string controller,string action,string id)
        {
            Dictionary<string, string> lRouteData = new Dictionary<string, string>();
            lRouteData.Add("controller", controller);
            lRouteData.Add("action", action);
            lRouteData.Add("id", id);

            Type lControllerType = FindController(controller);
            if (null == lControllerType)
            {
                throw new Exception("Controller " + controller + " not found.");
            }
            MethodInfo lAction = GetMethod(context, lControllerType, action);
            if (null == lAction)
            {
                throw new Exception("Action " + action + " not found.");
            }
            BaseProcessor processor = CreateProcessor(context);
            byte[] lRes = processor.Process(lControllerType, lAction, lRouteData);
            return lRes;
        }


        public virtual byte[] InvokeRequest(IHttpContextEx context)
        {
            string lPath = context.Request.Path;
            int idx = lPath.IndexOf(".mvc");
            string prefix = lPath.Substring(0, idx);
            int startIdx = prefix.LastIndexOf("/");
            lPath = lPath.Substring(startIdx + 1);

            string[] str = lPath.Split('/');
            string controller = str.Length > 0 ? str[0] : "Default";
            if (controller.Contains("."))
            {
                controller = controller.Remove(controller.LastIndexOf("."));
            }
            string action = str.Length > 1 ? str[1] : "Index";
            action = action.Contains("?") ? action.Remove(action.IndexOf('?')) : action;
            string id = str.Length > 2 ? str[2] : "";

            return InvokeRequest(context, controller, action, id);
        }

        private MethodInfo GetMethod(IHttpContextEx context, Type lType, string method)
        {
            MethodInfo lRes = null;

            foreach (MethodInfo lMethod in lType.GetMethods())
            {
                if (lMethod.Name == method && lMethod.IsPublic)
                {
                    if (context.Request.Method == "GET" && lMethod.GetParameters().Length == 0)
                    {
                        lRes = lMethod;
                        break;
                    }
                    else if (context.Request.Method == "POST" && lMethod.GetParameters().Length > 0)
                    {
                        lRes = lMethod;
                        break;
                    }
                }
            }

            return lRes;
        }

        private BaseProcessor CreateProcessor(IHttpContextEx context)
        {
            BaseProcessor processor = null;
            if ((!string.IsNullOrEmpty(context.Request.ContentType) && 
                context.Request.ContentType.Contains("application/json")) ||
                context.Request.RawUrl.Contains("Commet.mvc"))
            {
                processor = new AjaxProcessor(context);
            }
            else if (context.Request.Method == "POST")
            {
                processor = new AspxProcessor(context);
            }
            else if (context.Request.Method == "GET")
            {
                processor = new BaseProcessor(context);
            }
            Debug.Assert(null != processor);
            return processor;
        }

        private Type FindController(string lName)
        {
            string fullName = lName + "Controller";
            if (!AppAssembly.Inst().HasController(fullName))
            {
                throw new Exception("Controller " + fullName + " not found");
            }

            Type lRes = AppAssembly.Inst().GetControllerType(fullName);
            return lRes;
        }
	}
}
