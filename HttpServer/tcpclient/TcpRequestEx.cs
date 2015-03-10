using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Sockets;
using System.Text;
using CoreEx;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.tcpclient
{
    public class TcpRequestEx : IHttpRequestEx
    {
        private NameValueCollection _headers = new NameValueCollection();
        private NameValueCollectionEx _form = new NameValueCollectionEx();
        private NameValueCollection _queryString = new NameValueCollection();
        private MemoryStream _inputStream = new MemoryStream();

        private string _path = "";
        private string _rawUrl = "";
        private string _httpMethod = "";

        public TcpRequestEx(TcpListenerContext context)
        {
            TcpClient tcpClient = context._tcpClient;
            MemoryStream bufferStream = new MemoryStream();
            NetworkStream stream = tcpClient.GetStream();
            do
            {
                Byte[] bytes = new Byte[tcpClient.Available];
                stream.Read(bytes, 0, bytes.Length);
                bufferStream.Write(bytes, (int)bufferStream.Length, bytes.Length);
            }
            while (tcpClient.Available > 0) ;
            //translate bytes of request to string
            String received = Encoding.UTF8.GetString(bufferStream.GetBuffer());

            string[] rows = received.Split('\r');

            string row = rows[0];
            for (int i = 1; i < rows.Length; ++i)
            {
                row = rows[i];
                row = row.Replace("\n", "");
                int idx = row.IndexOf(':');
                if (idx > 0)
                {
                    string key = row.Substring(0, idx);
                    string val = row.Substring(idx + 1).TrimStart(new char[] { ' ' });
                    _headers.Add(key, val);
                }
            }

            string[] data = received.Split(new string[1] { "\r\n" }, StringSplitOptions.None);
            string[] firstUrl = data[0].Split(' ');

            _httpMethod = firstUrl[0];
            _rawUrl = firstUrl[1];
            _path = String.Format("http://{0}{1}", _headers["Host"], _rawUrl);
            if (string.IsNullOrEmpty(data[data.Length - 2]) && !string.IsNullOrEmpty(data[data.Length - 1]))
            {
                string content = data[data.Length - 1];
                byte[] buffer = Utils.DefaultEncoding.GetBytes(content);
                _inputStream.Write(buffer, 0, buffer.Length);
                _inputStream.Position = 0;
            }
           
        }

        public NameValueCollectionEx Form
        {
            get { return _form; }
        }

        public NameValueCollection QueryString
        {
            get { return _queryString; }
        }

        public string Path
        {
            get { return _path; }
        }

        public NameValueCollection Headers
        {
            get { return _headers; }
        }

        public string HttpMethod
        {
            get { return _httpMethod; }
        }

        public Stream InputStream
        {
            get { return _inputStream; }
        }

        public string ContentType
        {
            get { return _headers["Content-Type"]; }
        }

        public string RawUrl
        {
            get { return _rawUrl; }
        }

        public string MapPath(string virtualPath)
        {
            return "";
        }

        public object GetCookie(string p)
        {
            throw new NotImplementedException();
        }


        public string Method
        {
            get { return _httpMethod; }
        }

        public long ContentLength
        {
            get { return _inputStream.Length; }
        }

        public void Dispose()
        {
            _inputStream.Close();
            _inputStream = null;
        }

    }
}