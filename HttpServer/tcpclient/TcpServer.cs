using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CoreEx;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.tcpclient
{
    public class TcpListenerServer : HttpServerBase
    {
        private TcpListener _listener;
        private string _host;
        private string _port;
     
        public TcpListenerServer(string host,string port)
        {
            _host = host;
            _port = port;

            ThreadPool.SetMinThreads(100, 100);
            ThreadPool.SetMaxThreads(2000, 2000);

            _listener = new TcpListener(IPAddress.Parse(host), int.Parse(port));
        }

        public override void Start()
        {
            _listener.Start();
            Logger.Inst.Trace(String.Format("Tcp Server started at {0}:{1}",_host,_port));

            listen();
        }

        private void listen()
        {
            while (true)
            {
                IAsyncResult result = _listener.BeginAcceptTcpClient(new AsyncCallback(ListenerCallback), _listener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        private void ListenerCallback(IAsyncResult lResult)
        {
            TcpClient lTcpClient = _listener.EndAcceptTcpClient(lResult);
            IHttpContextEx lHttpContext = null;

           

            do
            {
                lHttpContext = new TcpListenerContext(lTcpClient);
            } 
            while (false);

            Process(lHttpContext);
        }

        public override void Stop()
        {
            _listener.Stop();
            Logger.Inst.Trace(String.Format("Server stopped"));
        }
    }

}
