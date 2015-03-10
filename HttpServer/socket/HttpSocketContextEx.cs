using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.socket
{
    public class HttpSocketContextEx : IHttpContextEx,IDisposable
    {
        public string SessionID { get; private set; }
        public IHttpRequestEx Request
        {
            get; set; }

        public IHttpResponseEx Response
        {
            get; set; 
        }

        public HttpSocketContextEx(Socket socket)
        {
            this.Socket = socket;
            this.Request = new HttpSocketRequestEx(this);
            this.Response = new HttpSocketResponseEx(this);
        }

        public void AddToSession(string key, object value)
        {
        }

        public void RemoveFromSession(string key)
        {
        }

        public void ServerExecute(string virtualPath, StringWriter sw)
        {
        }

        public string WebDir
        {
            get
            {
                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                DirectoryInfo projectDir = dir.Parent.Parent;
                DirectoryInfo[] webDirs = projectDir.GetDirectories("web");
                return webDirs[0].FullName;
            }
        }

        public Socket Socket { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            (this.Request as HttpSocketRequestEx).Dispose();
            (this.Response as HttpSocketResponseEx).Dispose();

            this.Request = null;
            this.Response = null;
        }

        #endregion

        #region IHttpContextEx Members

        public object GetFromSession(string key)
        {
            return null;
        }

        #endregion
    }
}
