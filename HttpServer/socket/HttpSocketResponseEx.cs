using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using CoreEx;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.socket
{
    public class HttpSocketResponseEx : IHttpResponseEx,IDisposable
    {
        private int _statusCode = 200;
        private Dictionary<string,string> _headers = new Dictionary<string, string>();
        private HttpSocketContextEx _context;

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

        public HttpSocketResponseEx(HttpSocketContextEx httpSocketContextEx)
        {
            _context = httpSocketContextEx;

            this._headers.Add("Cache-Control", "private");
            this._headers.Add("Vary", "Accept-Encoding");
            this._headers.Add("Date", "Tue, 25 Dec 2012 10:22:21 GMT");
       
        }

        public void WriteToOutputStream(string lData)
        {
            StringBuilder lRes = new StringBuilder();

            lRes.AppendFormat("HTTP/1.1 200 OK\n");
            foreach (KeyValuePair<string, string> keyValuePair in _headers)
            {
                lRes.AppendFormat("{0}: {1}\n", keyValuePair.Key, keyValuePair.Value);
            }
            //lRes.AppendFormat("Content-Length: {0}\n", lData.Length);
            lRes.AppendFormat("\n");
            lRes.AppendFormat("{0}", lData);
            string lResponse = lRes.ToString();

            Byte[] sendBuffer = Utils.DefaultEncoding.GetBytes(lResponse);
            _context.Socket.Send(sendBuffer);
            _context.Socket.Close();
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

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IHttpResponseEx Members

        public int ContentLength { get; set; }


        public void AddCookie(object cookieSession)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
