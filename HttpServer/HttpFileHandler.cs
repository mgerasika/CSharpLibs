using System;
using System.Collections.Generic;
using System.IO;

using System.Net;
using System.Text;
using CoreEx;
using MvcEx;
using MvcEx.httpclient;

namespace HttpServer
{
    public class HttpFileHandler : HttpHandlerBase
    {
        protected override void AddHeaders(IHttpResponseEx httpResponse)
        {
            base.AddHeaders(httpResponse);

            httpResponse.AppendHeader("Connection", "keep-alive");
            httpResponse.AppendHeader("Cache-Control", "max-age=31103031");
            httpResponse.AppendHeader("Expires", "Thu, 04 Apr 2015 14:47:59 GMT");
            httpResponse.AppendHeader("Vary", "Accept-Encoding");
            httpResponse.AppendHeader("Date", "Mon, 20 Jan 2014 20:09:57 GMT");
            httpResponse.AppendHeader("Last-Modified", "Wed, 15 Jan 2014 19:54:13 GMT");
        }
        public override void Process(IHttpContextEx httpContext)
        {
            byte[] fileContent = GetFileContent(httpContext);
            SendResponse(httpContext, fileContent);
        }

        protected byte[] GetFileContent(IHttpContextEx httpContext)
        {
            byte[] lRes = null;

            string lUrl = httpContext.Request.RawUrl.Remove(0, 1);
            int idx = lUrl.IndexOf("?");
            if(-1 != idx)
            {
                lUrl = lUrl.Remove(idx);
            }

#if !DEBUG
            if(CacheManager.Inst().HasItem(lUrl))
            {
               lRes = CacheManager.Inst().GetItem(lUrl).Content;
            }
            else
            {
               lock (this)
               {
                   lRes = ReadFile(lUrl, httpContext.WebDir);
                   CacheManager.Inst().AddItem(new CacheItem(){Url = lUrl,Content = lRes});
               }
            }
#else
            lRes = ReadFile(lUrl,httpContext.WebDir);
#endif

            return lRes;
        }

        protected byte[] ReadFile(string fileUrl,string siteDir)
        {
            byte[] lRes = null;

            string lFileName = GetFilePhysicalPath(fileUrl,siteDir);
            if (!string.IsNullOrEmpty(lFileName))
            {
                lock (this)
                {
                    using (FileStream fs = new FileStream(lFileName,FileMode.Open))
                    {
                        lRes = Utils.ReadStream(fs);
                    }
                }
            }

            return lRes;
        }

        private string GetFilePhysicalPath(string fileUrl,string siteDir)
        {
            string lFileName = "";

            try
            {
                DirectoryInfo web = new DirectoryInfo(siteDir);
                FileInfo[] files = web.GetFiles(fileUrl);
                if (files.Length == 1)
                {
                    FileInfo lFileInfo = files[0];
                    lFileName = lFileInfo.FullName;
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("File not found", ex);
            }

            return lFileName;
        }
    }
}
