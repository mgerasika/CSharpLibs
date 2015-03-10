using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using CoreEx;
using MvcEx;

namespace HttpServer
{
    public class HttpServiceHlp
    {
        private static object _lockObject = new object();
        private static HttpServiceHlp _instance;
        private string _serverUrl;

        private HttpServiceHlp()
        {
            string host = ConfigurationManager.AppSettings["httpServerIp"];
            Debug.Assert(!string.IsNullOrEmpty(host));
            string port = ConfigurationManager.AppSettings["httpServerPort"]; ;
            Debug.Assert(!string.IsNullOrEmpty(port));
            _serverUrl = String.Format("http://{0}:{1}/", host, port);
        }

        public ResponseBase Process(RequestBase request)
        {
            request.DebugID = Guid.NewGuid();

            ResponseBase response = null;
            HttpWebRequest httpRequest = HttpWebRequest.Create(_serverUrl) as HttpWebRequest;
            byte[] byteArray = Utils.Serialize(request);
            httpRequest.ContentLength = byteArray.Length;
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Method = "POST";
            Stream dataStream = httpRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            string responseData = "";
            using(StreamReader sr = new StreamReader(httpResponse.GetResponseStream()))
            {
                responseData = sr.ReadToEnd();
            }
            Debug.Assert(!string.IsNullOrEmpty(responseData));
            object responseObject = Utils.DeserializeStr(responseData);
            response = Utils.DeserializeObject(responseObject) as ResponseBase;
            Debug.Assert(response.DebugID == request.DebugID);

            return response;
        }

        public static HttpServiceHlp Inst()
        {
            if (null == _instance)
            {
                lock (_lockObject)
                {
                    if (null == _instance)
                    {
                        _instance = new HttpServiceHlp();
                    }
                }
            }
            return _instance;
        }
    }
}
