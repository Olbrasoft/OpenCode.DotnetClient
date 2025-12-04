using OpenCode.DotnetClient.Models;
using Refit;

namespace OpenCode.DotnetClient;

/// <summary>
/// Main client for interacting with OpenCode API
/// </summary>
public class OpenCodeClient : IDisposable
{
    private readonly IOpenCodeApi _api;
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;

    /// <summary>
    /// Creates a new OpenCodeClient instance
    /// </summary>
    /// <param name="baseUrl">Base URL of OpenCode server (default: http://localhost:4096)</param>
    public OpenCodeClient(string baseUrl = "http://localhost:4096")
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _disposeHttpClient = true;
        _api = RestService.For<IOpenCodeApi>(_httpClient);
    }

    /// <summary>
    /// Creates a new OpenCodeClient instance with a custom HttpClient
    /// </summary>
    /// <param name="httpClient">Custom HttpClient instance</param>
    public OpenCodeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _disposeHttpClient = false;
        _api = RestService.For<IOpenCodeApi>(_httpClient);
    }

    /// <summary>
    /// List all sessions
    /// </summary>
    public Task<List<Session>> GetSessionsAsync(string? directory = null)
        => _api.GetSessionsAsync(directory);

    /// <summary>
    /// Create a new session
    /// </summary>
    public Task<Session> CreateSessionAsync(string? title = null, string? parentId = null, string? directory = null)
    {
        var request = new CreateSessionRequest();
        // Only set properties if they are not null to avoid sending null values
        if (title != null || parentId != null)
        {
            request = new CreateSessionRequest
            {
                Title = title,
                ParentId = parentId
            };
        }
        return _api.CreateSessionAsync(request, directory);
    }

    /// <summary>
    /// Get a specific session
    /// </summary>
    public Task<Session> GetSessionAsync(string sessionId, string? directory = null)
        => _api.GetSessionAsync(sessionId, directory);

    /// <summary>
    /// Delete a session
    /// </summary>
    public Task<bool> DeleteSessionAsync(string sessionId, string? directory = null)
        => _api.DeleteSessionAsync(sessionId, directory);

    /// <summary>
    /// Send a text prompt to a session and wait for response
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="prompt">Text prompt to send</param>
    /// <param name="providerId">Provider ID (e.g., "anthropic")</param>
    /// <param name="modelId">Model ID (e.g., "claude-3-5-sonnet-20241022")</param>
    /// <param name="directory">Optional directory</param>
    public Task<PromptResponse> SendPromptAsync(
        string sessionId,
        string prompt,
        string providerId = "anthropic",
        string modelId = "claude-3-5-sonnet-20241022",
        string? directory = null)
    {
        var request = new PromptRequest
        {
            Parts = new List<MessagePart>
            {
                new MessagePart { Type = "text", Text = prompt }
            },
            Model = new ModelConfig
            {
                ProviderId = providerId,
                ModelId = modelId
            }
        };
        return _api.SendPromptAsync(sessionId, request, directory);
    }

    /// <summary>
    /// Send a prompt asynchronously (fire and forget)
    /// </summary>
    public Task SendPromptAsyncAsync(
        string sessionId,
        string prompt,
        string providerId = "anthropic",
        string modelId = "claude-3-5-sonnet-20241022",
        string? directory = null)
    {
        var request = new PromptRequest
        {
            Parts = new List<MessagePart>
            {
                new MessagePart { Type = "text", Text = prompt }
            },
            Model = new ModelConfig
            {
                ProviderId = providerId,
                ModelId = modelId
            }
        };
        return _api.SendPromptAsyncAsync(sessionId, request, directory);
    }

    /// <summary>
    /// List messages for a session
    /// </summary>
    public Task<List<MessageWithParts>> GetMessagesAsync(string sessionId, int? limit = null, string? directory = null)
        => _api.GetMessagesAsync(sessionId, limit, directory);

    /// <summary>
    /// Abort a running session
    /// </summary>
    public Task<bool> AbortSessionAsync(string sessionId, string? directory = null)
        => _api.AbortSessionAsync(sessionId, directory);

    /// <summary>
    /// Get todos for a session
    /// </summary>
    public Task<List<Todo>> GetTodosAsync(string sessionId, string? directory = null)
        => _api.GetTodosAsync(sessionId, directory);

    /// <summary>
    /// Create an event stream to listen for global events
    /// </summary>
    /// <returns>Event stream handler</returns>
    public OpenCodeEventStream CreateEventStream()
    {
        return new OpenCodeEventStream(_httpClient.BaseAddress?.ToString() ?? "http://localhost:4096");
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient?.Dispose();
        }
    }
}
