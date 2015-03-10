using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using CoreEx;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer.socket
{
    public class HttpSocketRequestEx : IHttpRequestEx,IDisposable
    {
        private NameValueCollection _headers = new NameValueCollection();
        private NameValueCollectionEx _form = new NameValueCollectionEx();
        private NameValueCollection _queryString = new NameValueCollection();
        private MemoryStream _inputStream = new MemoryStream();

        private string _path = "";
        private string _rawUrl = "";
        private string _httpMethod = "";

        public HttpSocketRequestEx(HttpSocketContextEx context)
        {
            byte[] buffer = new byte[context.Socket.ReceiveBufferSize];
            int requestLength = 0;
            do
            {
                requestLength = context.Socket.Receive(buffer);
            } while (false);


            byte[] newBuffer = new byte[requestLength];
            Array.Copy(buffer,0,newBuffer,0,requestLength);
            string received = Utils.DefaultEncoding.GetString(newBuffer);
            if (!string.IsNullOrEmpty(received))
            {
                string[] rows = received.Split('\r');

                string row = rows[0];
                for (int i = 1; i < rows.Length; ++i)
                {
                    row = rows[i];
                    row = row.Replace("\n", "");
                    requestLength = row.IndexOf(':');
                    if (requestLength > 0)
                    {
                        string key = row.Substring(0, requestLength);
                        string val = row.Substring(requestLength + 1).TrimStart(new char[] {' '});
                        _headers.Add(key, val);
                    }
                }

                string[] data = received.Split(new string[1] {"\r\n"}, StringSplitOptions.None);
                string[] firstUrl = data[0].Split(' ');

                _httpMethod = firstUrl[0];
                _rawUrl = firstUrl[1];
                _path = String.Format("http://{0}{1}", _headers["Host"], _rawUrl);
                if (string.IsNullOrEmpty(data[data.Length - 2]) && !string.IsNullOrEmpty(data[data.Length - 1]))
                {
                    string content = data[data.Length - 1];
                    buffer = Utils.DefaultEncoding.GetBytes(content);
                    _inputStream.Write(buffer, 0, buffer.Length);
                    _inputStream.Position = 0;
                }
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

        

        public void Dispose()
        {
            _inputStream.Close();
            _inputStream = null;
        }

        public string Method
        {
            get { return _httpMethod; }
        }

        public object GetCookie(string p)
        {
            throw new NotImplementedException();
        }

        public long ContentLength
        {
            get { return _inputStream.Length; }
        }
    }
}
