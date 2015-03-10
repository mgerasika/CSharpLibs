using System;
using System.Collections.Generic;

namespace HttpServer.websocket
{
    public class MyStream
    {
        public List<byte> Buffer = new List<byte>();

        public bool HasNext
        {
            get { return Buffer.Count > 0; }
        }

        public event EventHandler BufferChanged;

        internal void Write(byte[] arr, int start, int length)
        {
            lock (Buffer)
            {
                for (int i = start; i < length; ++i)
                {
                    Buffer.Add(arr[i]);
                }

                if (length > 0 && null != BufferChanged)
                {
                    BufferChanged(this, new EventArgs());
                }
            }
        }

        internal byte ReadNext()
        {
            byte res = 0;

            if (HasNext)
            {
                res = Buffer[0];
                lock (Buffer)
                {
                    Buffer.RemoveAt(0);
                }
            }
            return res;
        }

        internal bool NextIs(byte p)
        {
            bool res = false;

            if (HasNext)
            {
                res = (Buffer[0] == p);
            }
            return res;
        }
    }
}