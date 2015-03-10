using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web;
using CoreEx;
using MvcEx.httpclient;

namespace MvcEx
{
    public abstract class ControllerBase : IControllerBase, IObjectEx, IDisposable
    {
        private Dictionary<string, object> _viewData = new Dictionary<string, object>();

        public string _cstype
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public DataManagerBase DataManager
        {
            get
            {
                DataManagerBase lDataManager = null;
                if (null != System.Web.HttpContext.Current && null != System.Web.HttpContext.Current.Session)
                {
                    lDataManager = System.Web.HttpContext.Current.Session["DataManager"] as DataManagerBase;
                }
                else
                {
                    lDataManager = this.HttpContext.GetFromSession("DataManager") as DataManagerBase;
                }
                if (null == lDataManager) {
                    lDataManager = Factory.Inst().GetAbstactFactory().CreateDataManager();
                    if (null != System.Web.HttpContext.Current && null != System.Web.HttpContext.Current.Session) {
                        System.Web.HttpContext.Current.Session["DataManager"] = lDataManager;
                    }
                }
                return lDataManager;
            }
        }

        public IMsSqlConnection Connection
        {
            get { return this.DataManager.Connection; }
        }

        public Dictionary<string, string> RouteData { get; set; }

        public IHttpContextEx HttpContext { get; set; }
        
        public Dictionary<string, object> ViewData
        {
            get { return _viewData; }
        }

        public virtual void Dispose()
        {
        }

        public ViewResult RedirectToAction(string action,string controller)
        {
            return CreateView(action,controller);
        }

        public ViewResult View(string action)
        {
            int idx = this.GetType().Name.IndexOf("Controller");
            string controller = this.GetType().Name.Remove(idx);
            return CreateView(action, controller);
        }

        private ViewResult CreateView(string name,string controller)
        {
            ViewResult lViewResult = new ViewResult() { ControllerName = controller, ViewName = name };
            return lViewResult;
        }

        public JsonResult Json(object json)
        {
            JsonResult jRes = new JsonResult() { Json = json };
            return jRes;
        }

        public TextResult Text(string text)
        {
            TextResult jRes = new TextResult() { Text = text };
            return jRes;
        }

        public FileResult File(string file)
        {
            FileResult jRes = new FileResult() { FilePath = file };
            return jRes;
        }

        internal void InitTempData(IHttpContextEx context)
        {
            this.HttpContext = context;
            context.AddToSession("ViewData",this.ViewData);
        }

        internal void DeleteTempData(IHttpContextEx context)
        {
            context.RemoveFromSession("ViewData");

            this.HttpContext = null;
            this._viewData = null;
        }
    }
}
