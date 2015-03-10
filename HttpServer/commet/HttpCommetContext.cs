using System;
using System.Diagnostics;
using System.Text;
using CoreEx;
using MvcEx;
using System.Timers;
using MvcEx.httpclient;

namespace HttpServer.commet
{
    internal class HttpCommetContext
    {
        public IHttpContextEx HttpContext { get; set; }
        public HttpCommetHandler CommetHandler { get; set; }
        private Timer _timer = new Timer();

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            HttpCommetContext lCommetContext = this;
            HttpCommetManager.Inst().RemoveCommetContext(this);
            //Logger.Inst().Trace("Timer tick and " + ((lCommetContext != null) ? "items not null" : "null"));
            lCommetContext.CompleteRequest(null as object);
        }

        public HttpCommetContext(HttpCommetHandler commetHandler)
        {
            this.CommetHandler = commetHandler;

            _timer.Interval = 59 * 1000;
            _timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            _timer.Start();
        }

        private void ClearTimers()
        {
            _timer.Elapsed-= new ElapsedEventHandler(OnTimerElapsed);
            _timer.Stop();
            _timer.Dispose();
        }

        public void CompleteRequest(RequestBase request)
        {
           CompleteRequestInternal(request);
        }

        public void CompleteRequest(ResponseBase response)
        {
            CompleteRequestInternal(response);
        }

        public void CompleteRequest(object tempData)
        {
            CompleteRequestInternal(tempData);
        }

        private void CompleteRequestInternal(Object lData)
        {
            ClearTimers();

            this.HttpContext.AddToSession("TempData", lData);
            byte[] lRes = null;
            try
            {
                lRes = MvcProcessorEx.Inst().InvokeRequest(this.HttpContext);
            }
            catch(Exception ex)
            {
                this.HttpContext.Response.StatusCode = 500;
                lRes = Utils.DefaultEncoding.GetBytes(ex.ToString());
            }

            this.CommetHandler.CompleteRequest(this.HttpContext,lRes);
            this.HttpContext.RemoveFromSession("TempData");
        }
    }
}
