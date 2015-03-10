using System;
using System.IO;
using System.Net.Sockets;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.tcpclient
{
    public class TcpListenerContext : IHttpContextEx
    {
        public TcpClient _tcpClient { get; set; }

        public TcpListenerContext(TcpClient tcpClient)
        {
            this._tcpClient = tcpClient;

            this.Request = new TcpRequestEx(this);
            this.Response = new TcpResponseEx(this);
        }

        public IHttpRequestEx Request { get; set; }

        public IHttpResponseEx Response { get; set; }

        public void AddToSession(string key, object value)
        {
        }

        public void RemoveFromSession(string key)
        {
        }

        public object GetFromSession(string key)
        {
            return null;
        }

        public void ServerExecute(string virtualPath, StringWriter sw)
        {
        }

        public string SessionID
        {
            get { return null; }
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
    }
}