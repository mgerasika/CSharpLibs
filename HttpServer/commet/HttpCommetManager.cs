using System;
using System.Collections.Generic;
using System.Timers;
using CoreEx;
using MvcEx;
using System.Diagnostics;
using MvcEx.httpclient;

namespace HttpServer.commet
{
    public delegate bool CompareContextsPredicate(IHttpContextEx self,IHttpContextEx target);
    public class HttpCommetManager
    {
        private static object _lockObject = new object();
        private static HttpCommetManager _instance;
        private List<HttpCommetContext> _commetContexts = new List<HttpCommetContext>();
        private Queue<ObjectEx> _data = new Queue<ObjectEx>();

        private HttpCommetManager()
        {
        }

        public static HttpCommetManager Inst()
        {
            if (null == _instance)
            {
                lock (_lockObject)
                {
                    if (null == _instance)
                    {
                        _instance = new HttpCommetManager();
                    }
                }
            }
            return _instance;
        }

        internal HttpCommetContext GetCommetContext()
        {
            HttpCommetContext lRes = null;
            lock (_commetContexts)
            {
                if (_commetContexts.Count > 0)
                {
                    lRes = _commetContexts[0]; 
                    _commetContexts.RemoveAt(0);
                }
            }
            return lRes;
        }

        internal List<HttpCommetContext> GetCommetContextsByPredicate(IHttpContextEx context,CompareContextsPredicate fnCompare)
        {
            List<HttpCommetContext> lRes = new List<HttpCommetContext>();
            lock (_commetContexts)
            {
                for(int i=0;i<_commetContexts.Count;++i)
                {
                    HttpCommetContext item = _commetContexts[i];
                    if (fnCompare(context,item.HttpContext))
                    {
                        lRes.Add(item);
                        _commetContexts.RemoveAt(i);
                        i = i-1;
                    }
                }
            }
            return lRes;
        }

        internal ObjectEx GetDataItem()
        {
            ObjectEx lRes = null;
            lock (_data)
            {
                if (_data.Count > 0)
                {
                    lRes = _data.Dequeue();
                }
            }
            return lRes;
        }

        internal void AddCommetContext(HttpCommetContext lContext)
        {
            lock (_commetContexts)
            {
                _commetContexts.Add(lContext);
            }
        }

        internal void RemoveCommetContext(HttpCommetContext lContext)
        {
            lock (_commetContexts)
            {
                _commetContexts.Remove(lContext);
            }
        }

        public void SendToLast(ObjectEx lData)
        {
            HttpCommetContext lCommet = GetCommetContext();
            if (null != lCommet)
            {
                lCommet.CompleteRequest(lData);
            }
            else
            {
                lock (_data)
                {
                    _data.Enqueue(lData);
                }
            }
        }

        public void SendTo(object lData,IHttpContextEx context,CompareContextsPredicate fnCompare)
        {
            List<HttpCommetContext> lItems = GetCommetContextsByPredicate(context,fnCompare);
            foreach (HttpCommetContext httpCommetContext in lItems)
            {
                httpCommetContext.CompleteRequest(lData);    
            }
        }

        public void SendToAll(Object lData)
        {
            lock (_commetContexts)
            {
                do
                {
                    HttpCommetContext lCommet = GetCommetContext();
                    if (null == lCommet)
                    {
                        break;
                    }
                    lCommet.CompleteRequest(lData);
                } while (true);
            }
        }

         public bool HasCommetContext()
        {
            bool lRes = false;
            lock (_commetContexts)
            {
                if (_commetContexts.Count > 0)
                {
                    lRes = true;
                }
            }
            return lRes;
        }
    }
}
