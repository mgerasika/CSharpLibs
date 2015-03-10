using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreEx
{
    public class Utils
    {
        public static object _lockObject = new object();
        public static System.Text.Encoding DefaultEncoding = Encoding.Default;
        private static readonly JsonSerializer JsSerializer = new JsonSerializer();

        static Utils()
        {
            JsSerializer.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
        }
        public static string GetWebSiteContent(string url)
        {
            return GetWebSiteContent(url, Encoding.GetEncoding("Windows-1251"));
        }

        public static string GetWebSiteContent(string url,Encoding encoding)
        {
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream, encoding);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            return responseFromServer;
        }


        public static object DeserializeObject(Type type, object dictValue)
        {
            object lInstance = dictValue;

            if (type.IsSubclassOf(typeof(ObjectEx)))
            {
                lInstance = DeserializeObject(dictValue);
            }
            else
            {
                TypeConverter lTypeCoverter = TypeDescriptorEx.GetConverter(type);
                lInstance = lTypeCoverter.ConvertTo(dictValue, type);
            }
            return lInstance;
        }

        public static object DeserializeObject(object dictValue)
        {
            object lInstance = null;

            if(dictValue is JToken)
            {
                dictValue = (dictValue as JToken).ToObject<IDictionary>();
            }
            IDictionary dict = dictValue as IDictionary;
            Debug.Assert(null != dict);
            Debug.Assert(dict.Contains("_cstype"));
            string sInstanceType = dict["_cstype"].ToString();

            Type lInstanceType = AppAssembly.Inst().GetObjectType(sInstanceType);
            Debug.Assert(null != lInstanceType);
            ObjectEx lObj = Activator.CreateInstance(lInstanceType) as ObjectEx;
            lObj.Deserialize(dict);
            lInstance = lObj;

            return lInstance;
        }
        
        public static bool EqualsDate(DateTime dt1,DateTime dt2)
        {
            bool res = (dt1.Year == dt2.Year) && (dt1.Month == dt2.Month) && (dt1.Date == dt2.Date);
            return res;
        }

        

        public static string HtmlDecode(string str)
        {
            if (str == null)
                return String.Empty;
            str = HttpContext.Current.Server.HtmlDecode(str);
            str = str.Replace("'", "&#39;");
            str = str.Replace("\"", "&quot;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            /*
            if (str.Contains("\r"))
            {
                str = str.Replace("\r", "<br/>");
                str = str.Replace("\n", "");
            }
            else
            {
                str = str.Replace("\n", "<br/>");
                str = str.Replace("\r", "");
            }
            if (str.Length>5 &&(str.Length-5 == str.LastIndexOf("<br/>")))
            {
                str = str.Remove(str.Length - 5);
            }
            */
            return str;
        }

        public static bool IsGuid(string str)
        {
            bool res = false;
            try
            {
                Guid guid = new Guid(str);
                res = true;
            }
            catch (Exception ex)
            {
            }
            return res;
        }

        public static Guid ToGuid(string str)
        {
            Guid guid = new Guid(str);
            return guid;
        }

        public static String RenderJavaScript(string script)
        {
            string str = String.Format("<script type='text/javascript'>{0}</script>", script);
            return str;
        }

        

        public static byte[] Serialize(object request)
        {
            string requestString = SerializeStr(request);
            Byte[] sendBytes = Utils.DefaultEncoding.GetBytes(requestString);
            return sendBytes;
        }

        public static object Deserialize(byte[] receiveBytes,Type t)
        {
            string responseString = Utils.DefaultEncoding.GetString(receiveBytes);
            object response = DeserializeStr(responseString);
            return response;
        }

        public static string SerializeStr(object obj)
        {
            TextWriter textWriter = new StringWriter();
            JsSerializer.Serialize(textWriter,obj);
            return textWriter.ToString();
        }

        public static object DeserializeStr(string str)
        {
            TextReader textReader = new StringReader(str);
            JsonReader jsonReader = new JsonTextReader(textReader);
            object response = JsSerializer.Deserialize(jsonReader,typeof(IDictionary));
            Debug.Assert(null != response);
            return response;
        }

        public static string HashString(string rawValue)
        {
            SHA1 lSha1 = SHA1.Create();
            byte[] lHash = lSha1.ComputeHash(Encoding.Unicode.GetBytes(rawValue));
            return Convert.ToBase64String(lHash);
        }

        public static byte[] ReadStream(Stream lStream)
        {
            byte[] buffer = new byte[lStream.Length];
            int res = lStream.Read(buffer, 0, (int)lStream.Length);  
            Debug.Assert(lStream.Position == res);
            
            return buffer;
        }
    }

}//UtilsEx
