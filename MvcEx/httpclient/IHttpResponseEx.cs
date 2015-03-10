namespace MvcEx.httpclient
{
    public interface IHttpResponseEx
    {
        int StatusCode { get; set; }

        string ContentType { get; set; }
        int ContentLength { get; set; }

        void WriteToOutputStream(byte[] lRes);

        void AppendHeader(string key, string val);

        void CloseStream();

        void AddCookie(object cookieSession);
    }
}