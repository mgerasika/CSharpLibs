using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace HttpServer.websocket
{
    public delegate void WSClientDataReceived(object sender, WSClientDataEventArgs e);
    public class WebSocketManager
    {
        public static WebSocketManager Inst = new WebSocketManager();
        private List<WebSocketClient> _clients = new List<WebSocketClient>();
        public event WSClientDataReceived DataReceived;
        public event EventHandler ClientAdded;
        public event EventHandler ClientRemoved;

        public void AddClient(Socket clientSocket)
        {
            WebSocketClient webSocketClient = new WebSocketClient(clientSocket);
            lock (this._clients)
            {
                this._clients.Add(webSocketClient);
            }
            webSocketClient.Listen();
            webSocketClient.DataReceived += webSocketClient_DataReceived;

            if (null != this.ClientAdded)
            {
                this.ClientAdded(webSocketClient,new EventArgs());
            }
        }

        private void webSocketClient_DataReceived(object sender, WSClientDataEventArgs e)
        {
            if (null != DataReceived)
            {
                DataReceived(sender, e);
            }
        }

        public void Send(string msg,string clientId)
        {
            WebSocketClient webSocketClient = GetClient(clientId);
            webSocketClient.Send(msg);
        }

        public void SendToAll(string msg)
        {
            lock (this._clients)
            {
                foreach (WebSocketClient client in this.GetConnectedClients())
                {
                    client.Send(msg);    
                }
            }
        }

        private List<WebSocketClient> GetConnectedClients()
        {
            lock (this._clients)
            {
                for (int i = 0; i < this._clients.Count; ++i)
                {
                    WebSocketClient client = this._clients[i];
                    if (!client.IsConnected)
                    {
                         _clients.RemoveAt(i);
                        i--;
                    }
                }
            }
            return _clients;
        }

        public WebSocketClient GetClient(string clientId)
        {
            WebSocketClient webSocketClient = null;
            lock (this._clients)
            {
                foreach (WebSocketClient client in GetConnectedClients())
                {
                    if (client.ClientId.Equals(clientId))
                    {
                        webSocketClient = client;
                        break;
                    }
                }
            }
            return webSocketClient;
        }

        internal bool IsSockedUsed(Socket clientSocket)
        {
            bool res = false;
            lock (this._clients)
            {
                foreach (WebSocketClient client in this.GetConnectedClients())
                {
                    if (client.EqualsSocket(clientSocket))
                    {
                        res = true;
                        break;
                    }
                }
            }
            return res;
        }
    }
}