using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HttpServer.tcpclient;
using MvcEx;
using MvcEx.httpclient;
using CoreEx;

namespace HttpServer.handlers
{
    class WebSocketHandler : HttpHandlerBase
    {
        public override void Process(IHttpContextEx httpContext)
        {
            base.Process(httpContext);

            tcpclient.TcpListenerContext context = httpContext as TcpListenerContext;
            string data = "";
            if (new Regex("^GET").IsMatch(data))
            {
                Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
                    + "Connection: Upgrade" + Environment.NewLine
                    + "Upgrade: websocket" + Environment.NewLine
                    + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                        SHA1.Create().ComputeHash(
                            Encoding.UTF8.GetBytes(
                                new Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                            )
                        )
                    ) + Environment.NewLine
                    + Environment.NewLine);

                context._tcpClient.GetStream().Write(response, 0, response.Length);
            }

            SendResponse(httpContext, Utils.DefaultEncoding.GetBytes(this.Message));
        }

        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);

            httpResponse.StatusCode = 200;
            httpResponse.ContentType = "text/html";
        }

        public string Message { get; set; }

        public string InnerMessage { get; set; }
    }
}
