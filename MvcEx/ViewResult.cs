using System;
using System.Text;
using System.IO;
using System.Web;
using CoreEx;
using MvcEx.httpclient;

namespace MvcEx
{
    public abstract class ViewResultBase
    {
        public abstract byte[] CreateResult(IHttpContextEx context);
    }

    public class ViewResult : ViewResultBase
    {
        public string ControllerName { get; set; }
        public string ViewName { get; set; }

        public override byte[] CreateResult(IHttpContextEx context)
        {
            string lRes = "";

            string virtualPath = "Views/" + this.ControllerName + "/" + this.ViewName + ".aspx";
            string fileName = context.Request.MapPath(virtualPath);
            if (!File.Exists(fileName))
            {
                throw new Exception("File " + fileName + " not found");
            }

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                context.ServerExecute(virtualPath, sw);
                lRes = sb.ToString();
            }
            context.Response.ContentType = "text/html";
            return Utils.DefaultEncoding.GetBytes(lRes);
        }
    }

    public class JsonResult : ViewResultBase
    {
        public object Json { get; set; }
        public override byte[] CreateResult(IHttpContextEx context)
        {
            string lRes = Utils.SerializeStr(this.Json);
            //string buffer = Newtonsoft.Json.JsonConvert.SerializeObject(this.Json);
            context.Response.ContentType = "application/json";
            return Encoding.UTF8.GetBytes(lRes);
        }
    }

    public class TextResult : ViewResultBase
    {
        public string Text { get; set; }
        public override byte[] CreateResult(IHttpContextEx context)
        {
            string lRes = this.Text;
            //string buffer = Newtonsoft.Json.JsonConvert.SerializeObject(this.Json);
            context.Response.ContentType = "text/html";
            return Encoding.UTF8.GetBytes(lRes);
        }
    }

    public class FileResult : ViewResultBase
    {
        public string FilePath { get; set; }
        public override byte[] CreateResult(IHttpContextEx context)
        {
            byte[] lRes = null;
            lock(this)
            {
                using (FileStream fileStream = new FileStream(this.FilePath,FileMode.Open))
                {
                    lRes = Utils.ReadStream(fileStream);
                }
            }
            //context.Response.ContentType = "application/json";
            return lRes;
        }
    }
}
