using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using HttpServer.httplistener;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
{
    public class SessionItem 
    {
        private IDictionary<string,object> _session = new Dictionary<string, object>();
        public string ID { get; set; }

        public SessionItem()
        {
            _session["DataManager"] = Factory.Inst().GetAbstactFactory().CreateDataManager();
        }

        internal IHttpContextEx CreateContext(HttpListenerContext lContext)
        {
            IHttpContextEx lHttpContext = new HttpListenerContextEx(lContext,this._session,this.ID);
            return lHttpContext;
        }
    }
}
