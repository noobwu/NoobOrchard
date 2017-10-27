namespace Orchard.Client.Web
{
    public interface IHttpError : IHttpResult
    {
        string Message { get; }
        string ErrorCode { get; }
        string StackTrace { get; }
    }
}