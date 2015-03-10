using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using CoreEx;
using MvcEx.httpclient;

namespace MvcEx
{
    internal class BaseProcessor
    {
        protected IHttpContextEx HttpContext { get; set; }

        public BaseProcessor(IHttpContextEx context)
        {
            this.HttpContext = context;
        }

        public byte[] Process(Type controller, MethodInfo method, Dictionary<string, string> routeData)
        {
            byte[] res;
            
            using (ControllerBase lControllerInstance = Factory.Inst().GetAbstactFactory().CreateController(this.HttpContext,controller))
            {
                lControllerInstance.RouteData = routeData;
                res = InvokeMethod(lControllerInstance, method, null);
            }

            return res;
        }

        protected virtual byte[] InvokeMethod(ControllerBase lControllerInstance, MethodInfo method, object[] lParams)
        {
            byte[] res;
            
            using ( new TempDataInitializer(lControllerInstance,this.HttpContext))
            {
                ViewResultBase viewResult = method.Invoke(lControllerInstance, lParams) as ViewResultBase;
                res = viewResult.CreateResult(this.HttpContext);
                lControllerInstance.DeleteTempData(this.HttpContext);
            }
            return res;
        }

        private class TempDataInitializer : IDisposable
        {
            private IHttpContextEx HttpContext { get; set; }
            private ControllerBase ControllerInstance { get; set; }

            public TempDataInitializer(ControllerBase lControllerInstance,IHttpContextEx context)
            {
                this.ControllerInstance = lControllerInstance;
                this.HttpContext = context;
                this.ControllerInstance.InitTempData(this.HttpContext);
            }
            public void Dispose()
            {
                this.ControllerInstance.DeleteTempData(this.HttpContext);
                this.ControllerInstance = null;
                this.HttpContext = null;
            }
        }
    }

    internal class AjaxProcessor : BaseProcessor
    {
        public AjaxProcessor(IHttpContextEx context) : base(context) { }

        protected override byte[] InvokeMethod(ControllerBase lControllerInstance, MethodInfo method, object[] lParams)
        {
            string json = GetJson(this.HttpContext);
            lParams = GetParameters(method.GetParameters(), json);
            byte[] lRes = base.InvokeMethod(lControllerInstance, method, lParams);
            return lRes;
        }

        private string GetJson(IHttpContextEx context)
        {
            string json = "";
            if (context.Request.InputStream != null)
            {
                using (StreamReader reader = new StreamReader(context.Request.InputStream))
                {
                    json = reader.ReadToEnd();
                }
            }
            return json;
        }

        private object[] GetParameters(ParameterInfo[] lParameters, string lJson)
        {
            object[] lRes = null;

            if (!string.IsNullOrEmpty(lJson))
            {
                IDictionary lDict = Utils.DeserializeStr(lJson) as IDictionary;
                lRes = new object[lDict.Count];
                int i = 0;
                foreach (DictionaryEntry entry in lDict)
                {
                    ParameterInfo lParameter = lParameters[i];
                    lRes[i] = Utils.DeserializeObject(lParameter.ParameterType, entry.Value);
                    i++;
                }
            }

            return lRes;
        }
    }

    internal class AspxProcessor : BaseProcessor
    {
        public AspxProcessor(IHttpContextEx context) : base(context) { }

        protected override byte[] InvokeMethod(ControllerBase lControllerInstance, MethodInfo method, object[] lParams)
        {
            lParams = GetParameters(method.GetParameters(), this.HttpContext.Request.Form);
            byte[] lRes = base.InvokeMethod(lControllerInstance, method, lParams);
            return lRes;
        }

        private object[] GetParameters(ParameterInfo[] lParameters, NameValueCollectionEx lPost)
        {
            object[] lRes = new object[lParameters.Length];

            if (lParameters.Length > 0)
            {
                int i = 0;
                foreach (ParameterInfo lParameter in lParameters)
                {
                    object lVal = lPost.Get(lParameter.Name);
                    if (null != lVal)
                    {
                        lRes[i] = Utils.DeserializeObject(lParameter.ParameterType,lVal);
                    }
                    i++;
                }
            }

            return lRes;
        }
    }
}
