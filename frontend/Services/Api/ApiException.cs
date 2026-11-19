using System.Net;

namespace insureflow_portal.Services.Api;

public sealed class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public string? ResponseContent { get; }

    public ApiException(
        string message,
        HttpStatusCode statusCode,
        string? responseContent = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
}