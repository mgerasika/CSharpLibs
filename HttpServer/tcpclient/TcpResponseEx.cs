using System;
using System.Collections.Generic;
using System.Text;
using CoreEx;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.tcpclient
{
    public class TcpResponseEx : IHttpResponseEx
    {
        private int _statusCode = 200;
        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        private TcpListenerContext _context;

        public TcpResponseEx(TcpListenerContext context)
        {
            _context = context;

            this._headers.Add("Cache-Control", "private");
            this._headers.Add("Vary", "Accept-Encoding");
            this._headers.Add("Date", "Tue, 25 Dec 2012 10:22:21 GMT");
       
        }

        #region IHttpResponseEx Members

        public int StatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        public string ContentType
        {
            get { return _headers["Content-Type"]; }
            set { _headers["Content-Type"] = value; }
        }


        public int ContentLength { get; set; }

        public void WriteToOutputStream(string lData)
        {
            StringBuilder lRes = new StringBuilder();

            lRes.AppendFormat(String.Format("HTTP/1.1 {0}\n",this.StatusCode));
            foreach (KeyValuePair<string, string> keyValuePair in _headers)
            {
                lRes.AppendFormat("{0}: {1}\n", keyValuePair.Key, keyValuePair.Value);
            }
            //lRes.AppendFormat("Content-Length: {0}\n", lData.Length);
            lRes.AppendFormat("\n");
            lRes.AppendFormat("{0}", lData);
            string lResponse = lRes.ToString();

            Byte[] sendBuffer = Utils.DefaultEncoding.GetBytes(lResponse);
            this._context._tcpClient.GetStream().Write(sendBuffer, 0, sendBuffer.Length);
            this._context._tcpClient.Close();
        }

        public void WriteToOutputStream(byte[] lRes)
        {
            string line = Utils.DefaultEncoding.GetString(lRes);
            WriteToOutputStream(line);
        }

        public void AppendHeader(string key, string val)
        {
            _headers[key] = val;
        }

        public void CloseStream()
        {
        }

        public void AddCookie(object cookieSession)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}