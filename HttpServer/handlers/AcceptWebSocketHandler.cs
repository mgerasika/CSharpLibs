using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using HttpServer.socket;
using MvcEx;
using MvcEx.httpclient;
using CoreEx;

namespace HttpServer.handlers
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
            /*
            IHttpResponseEx httpResponse = httpContext.Response;
            httpResponse.AppendHeader("HTTP", "/1.1 101 Web Socket Protocol Handshake");
            httpResponse.AppendHeader("Upgrade", "websocket");
            httpResponse.AppendHeader("Connection", "upgrade");
            string path = this.HttpContext.Request.Path.Replace("http://", "");
            httpResponse.AppendHeader("Sec-WebSocket-Accept", "HSmrc0sMlYUkAGmm5OPpG2HaGWk=");
            httpResponse.AppendHeader("WebSocket-Origin", "http://" + path);
            httpResponse.AppendHeader("WebSocket-Location", "ws://" + path + "demo");
            httpResponse.AppendHeader("Sec-WebSocket-Protocol", "sample");
            httpResponse.WriteToOutputStream(new byte[0]);
             * */
        }

      

        private static string AcceptKey(string key)
        {
            string longKey = key + _guid;
            byte[] hashBytes = ComputeHash(longKey);
            return Convert.ToBase64String(hashBytes);
        }

        static SHA1 sha1 = SHA1CryptoServiceProvider.Create();
        private static byte[] ComputeHash(string str)
        {
            return sha1.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
        }
    }
}