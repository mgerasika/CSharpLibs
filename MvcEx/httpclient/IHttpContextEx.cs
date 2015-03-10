namespace MvcEx.httpclient
{
    public interface IHttpContextEx
    {
        IHttpRequestEx Request { get; set; }
        IHttpResponseEx Response { get; set; }

        void AddToSession(string key, object value);
        void RemoveFromSession(string key);
        object GetFromSession(string key);
        void ServerExecute(string virtualPath, System.IO.StringWriter sw);
        string SessionID { get; }
        string WebDir { get; }
    }
}