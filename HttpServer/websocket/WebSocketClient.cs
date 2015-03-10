using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HttpServer.websocket
{
    public class WSClientDataEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class WebSocketClient
    {
        private const int BUFFER_SIZE = 1000;
        private readonly byte[] _buffer = new byte[BUFFER_SIZE];
        private readonly Socket _clientSocket;
        private MyStream _stream;

        public WebSocketClient(Socket socket)
        {
            _clientSocket = socket;
            _clientSocket.SendTimeout = 6 *1000;
            _clientSocket.ReceiveTimeout = 6 *1000;
            _stream = new MyStream();
            this.ClientId = Guid.NewGuid().ToString();
        }

        public string ClientId { get; private set; }
        public event WSClientDataReceived DataReceived;

        internal void Listen()
        {
            Output_Run();
            Input_Run();
        }

        private void Input_Run()
        {
            lock (_clientSocket)
            {
                _clientSocket.BeginReceive(_buffer, 0, BUFFER_SIZE, 0, Read_Callback, this);
            }
        }

        private void Output_Run()
        {
            _stream.BufferChanged+=StreamOnBufferChanged;
        }

        private void StreamOnBufferChanged(object sender, EventArgs eventArgs)
        {
            //Array.Copy(_buffer, 0, data, 0, length);
            byte[] bytes = GetPacketFromStream();
            if (bytes != null && bytes.Length > 0)
            {
                string strContent = Encoding.ASCII.GetString(bytes);
                Console.WriteLine("Read {0} byte from socket data = {1} ", strContent.Length, strContent);
                if (null != DataReceived)
                {
                    var e = new WSClientDataEventArgs();
                    e.Message = strContent;

                    DataReceived(this, e);
                }
            }
            if (_stream.HasNext)
            {
               StreamOnBufferChanged(sender, eventArgs);
            }
        }

        private void Read_Callback(IAsyncResult ar)
        {
            lock (_clientSocket)
            {
                int length = _clientSocket.EndReceive(ar);
                if (length > 0)
                {
                    //_stream.Write(_buffer, 0, length);
                    _buffer[0] = 0;
                }
                else
                {
                    if (_buffer[0] != 0) {
                        
                    }
                }
            }
            lock(_clientSocket)
            {
                if (_clientSocket.Connected )
                {
                    try
                    {
                        _clientSocket.BeginReceive(_buffer, 0, BUFFER_SIZE, 0, Read_Callback, _clientSocket);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public void Send(string msg)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(msg);
            inputBytes = SendToClient(inputBytes);
            lock (_clientSocket)
            {
                _clientSocket.Send(inputBytes);
            }
        }

        private byte[] GetPacketFromStream()
        {
            byte[] decoded = null;

            byte[] bytes = _stream.Buffer.ToArray();
            if (bytes.Length > 0)
            {
                Byte secondByte = bytes[1];
                Int32 dataLength = secondByte & 127;
                Int32 indexFirstMask = 2;
                if (dataLength == 126)
                    indexFirstMask = 4;
                else if (dataLength == 127)
                    indexFirstMask = 10;

                IEnumerable<Byte> keys = bytes.Skip(indexFirstMask).Take(4);
                Int32 indexFirstDataByte = indexFirstMask + 4;
                if (indexFirstDataByte + dataLength <= bytes.Length)
                {
                    decoded = new Byte[dataLength];
                    for (Int32 i = indexFirstDataByte, j = 0; i < indexFirstDataByte + dataLength; i++, j++)
                    {
                        decoded[j] = (Byte) (bytes[i] ^ keys.ElementAt(j%4));
                    }
                    _stream.Buffer.RemoveRange(0, indexFirstDataByte + dataLength);
                }
            }
            return decoded;
        }

        private byte[] SendToClient(byte[] input)
        {
            Byte[] response;
            var frame = new Byte[10];

            Int32 indexStartRawData = -1;
            Int32 length = input.Length;

            frame[0] = 129;
            if (length <= 125)
            {
                frame[1] = (Byte) length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = 126;
                frame[2] = (Byte) ((length >> 8) & 255);
                frame[3] = (Byte) (length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = 127;
                frame[2] = (Byte) ((length >> 56) & 255);
                frame[3] = (Byte) ((length >> 48) & 255);
                frame[4] = (Byte) ((length >> 40) & 255);
                frame[5] = (Byte) ((length >> 32) & 255);
                frame[6] = (Byte) ((length >> 24) & 255);
                frame[7] = (Byte) ((length >> 16) & 255);
                frame[8] = (Byte) ((length >> 8) & 255);
                frame[9] = (Byte) (length & 255);

                indexStartRawData = 10;
            }

            response = new Byte[indexStartRawData + length];

            Int32 i, reponseIdx = 0;

            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = input[i];
                reponseIdx++;
            }

            return response;
        }

        internal void Close()
        {
            lock (_clientSocket)
            {
                _clientSocket.Disconnect(true);
                _clientSocket.Close();
            }
        }

        internal bool EqualsSocket(Socket clientSocket)
        {
            return clientSocket.Equals(_clientSocket);
        }

        public bool IsConnected
        {
            get { return _clientSocket.Connected; }
        }
    }
}