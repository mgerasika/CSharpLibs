using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading;
using CoreEx;
using HttpServer.thread;
using MvcEx;
using System.Net.Sockets;
using MvcEx.httpclient;

namespace HttpServer.socket
{
    public class HttpSocketServer : HttpServerBase
    {
        private IPEndPoint _endPoint;
        private Socket _socket;
        private CustomThreadManager<Socket> _threadManager;

        public HttpSocketServer()
            : this(ConfigurationManager.AppSettings["httpServerIp"],
            ConfigurationManager.AppSettings["httpServerPort"])
        {
        }

        public HttpSocketServer(string host, string port)
        {
            int lPort = int.Parse(port);
            _endPoint = new IPEndPoint(IPAddress.Parse(host), lPort);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _threadManager = new CustomThreadManager<Socket>(ListenerCallback,20);
        }

        public override void Start()
        {
            _socket.Bind(_endPoint);
            _socket.Listen(_endPoint.Port);
            _socket.BeginAccept(AcceptCallback, _socket);
            
            Logger.Inst.Trace(String.Format("Socket server started at {0}", _endPoint.ToString()));
            Console.WriteLine("Press any key to stop server");
            Console.Read();

            _socket.Close();
        }

        private void AcceptCallback(IAsyncResult result)
        {
            Socket server = (Socket)result.AsyncState;
            Socket client = server.EndAccept(result);
            _threadManager.Invoke(client);

            server.BeginAccept(AcceptCallback, server); // <- continue accepting connections
        }

        private void ListenerCallback(Socket client)
        {
            IHttpContextEx lHttpContext = new HttpSocketContextEx(client);
            Process(lHttpContext);
        }

        public override void Stop()
        {
            _socket.Close();
        }
    }
}
