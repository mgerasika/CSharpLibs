using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MvcEx;

namespace HttpServer
{
    public class SessionManager
    {
        private static SessionManager _instance = new SessionManager();
        private Dictionary<string,SessionItem> _sessions = new Dictionary<string, SessionItem>();
        private static object _lockobject = new object();

        private SessionManager()
        {
        }

        public SessionItem GetSession(string uniqueId)
        {
            SessionItem lSessionItem = null;
            
            if (_sessions.ContainsKey(uniqueId))
            {
                lSessionItem = _sessions[uniqueId];
            }
            else
            {
                lSessionItem = new SessionItem();
                lSessionItem.ID = uniqueId;
                //_sessions.Add(uniqueId, lSessionItem);
                
            }

            return lSessionItem;
        }

        public static SessionManager Inst()
        {
            return _instance;
        }
    }
}
