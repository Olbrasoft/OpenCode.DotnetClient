using System.Net;

namespace OpenCode.DotnetClient;

/// <summary>
/// Base exception for OpenCode API errors
/// </summary>
public class OpenCodeException : Exception
{
    /// <summary>
    /// HTTP status code if applicable
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Response content from the server
    /// </summary>
    public string? ResponseContent { get; }

    public OpenCodeException(string message) : base(message)
    {
    }

    public OpenCodeException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public OpenCodeException(string message, HttpStatusCode statusCode, string? responseContent = null) : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }

    public OpenCodeException(string message, HttpStatusCode statusCode, string? responseContent, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
}

/// <summary>
/// Exception thrown when the OpenCode server is not reachable
/// </summary>
public class OpenCodeConnectionException : OpenCodeException
{
    public OpenCodeConnectionException(string message) : base(message)
    {
    }

    public OpenCodeConnectionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when the API returns an error response
/// </summary>
public class OpenCodeApiException : OpenCodeException
{
    public OpenCodeApiException(string message, HttpStatusCode statusCode, string? responseContent = null)
        : base(message, statusCode, responseContent)
    {
    }

    public OpenCodeApiException(string message, HttpStatusCode statusCode, string? responseContent, Exception innerException)
        : base(message, statusCode, responseContent, innerException)
    {
    }
}
