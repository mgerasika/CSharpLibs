using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using CoreEx;
using HttpServer.socket;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.websocket
{
    public class AcceptWebSocketHandler : HttpHandlerBase
    {
        private static string _guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        public override void Process(IHttpContextEx httpContext)
        {
            base.Process(httpContext);

            HttpSocketContextEx context = httpContext as HttpSocketContextEx;
            Socket clientSocket = context.Socket;
            
            string acceptKey = AcceptKey(httpContext.Request.Headers["Sec-WebSocket-Key"]);
            string res =
                "HTTP/1.1 101 Switching Protocols\r\n" +
                "Upgrade: websocket\r\n" +
                "Connection: Upgrade\r\n" +
                "Sec-WebSocket-Accept: " + acceptKey + "\r\n";

            if (!string.IsNullOrEmpty(httpContext.Request.Headers["Sec-WebSocket-Protocol"]))
            {
                res += "Sec-WebSocket-Protocol: " + httpContext.Request.Headers["Sec-WebSocket-Protocol"] + "\r\n";
            }
            res += "\r\n";
            clientSocket.Send(Utils.DefaultEncoding.GetBytes(res));

            if (!WebSocketManager.Inst.IsSockedUsed(clientSocket))
            {
                WebSocketManager.Inst.AddClient(clientSocket);
            }
        }

        private string AcceptKey(string key)
        {
            string longKey = key + _guid;
            byte[] hashBytes = ComputeHash(longKey);
            return Convert.ToBase64String(hashBytes);
        }

        private SHA1 _sha1 = SHA1CryptoServiceProvider.Create();
        private byte[] ComputeHash(string str)
        {
            
                return _sha1.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
        }
    }
}