using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpServer
{
    delegate void CloseClientSocketHandler(Token token, SocketAsyncEventArgsEx e);

    public class SocketAsyncEventArgsEx : SocketAsyncEventArgs
    {
        internal event CloseClientSocketHandler CloseClientSocket;
        public event EventHandler<SocketAsyncEventArgsEx> Completed;

        protected override void OnCompleted(SocketAsyncEventArgs e)
        {
            base.OnCompleted(e);
            if(null != this.Completed)
            {
                this.Completed(this, this);
            }
        }

        internal void OnIOCompleted(object sender, SocketAsyncEventArgsEx e)
        {
            // Determine which type of operation just completed and call the associated handler.
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive();
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend();
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        internal void ProcessReceive()
        {
            if (this.BytesTransferred > 0)
            {
                if (this.SocketError == SocketError.Success)
                {
                    Token lToken = this.UserToken as Token;
                    lToken.Initialize(this);

                    Socket lSocket = lToken.Connection;
                    if (lSocket.Available == 0)
                    {
                        bool isCommet = lToken.ProcessData(this);
                        if (!isCommet)
                        {
                            if (!lSocket.SendAsync(this))
                            {
                                this.ProcessSend();
                            }
                        }
                    }
                    else if (!lSocket.ReceiveAsync(this))
                    {
                        this.ProcessReceive();
                    }
                }
                else
                {
                    this.ProcessError();
                }
            }
            else
            {
                if (null != this.CloseClientSocket)
                {
                    this.CloseClientSocket(this.UserToken as Token, this);
                }
            }
        }

        internal void ProcessSend()
        {
            if (this.SocketError == SocketError.Success)
            {
                Token token = this.UserToken as Token;
                if (!token.Connection.ReceiveAsync(this))
                {
                    this.ProcessReceive();
                }
            }
            else
            {
                this.ProcessError();
            }
        }

        internal void ProcessError()
        {
            Token token = this.UserToken as Token;
            IPEndPoint localEp = token.Connection.LocalEndPoint as IPEndPoint;

            if (null != this.CloseClientSocket)
            {
                this.CloseClientSocket(token, this);
            }

            Console.WriteLine("Socket error {0} on endpoint {1} during {2}.", (Int32)this.SocketError, localEp, this.LastOperation);
        }

    }
}
