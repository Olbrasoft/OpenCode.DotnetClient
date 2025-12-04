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
    private readonly OpenCodeClientOptions _options;

    /// <summary>
    /// Creates a new OpenCodeClient instance with custom options
    /// </summary>
    /// <param name="options">Client configuration options</param>
    public OpenCodeClient(OpenCodeClientOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl),
            Timeout = options.Timeout
        };
        _disposeHttpClient = true;
        _api = RestService.For<IOpenCodeApi>(_httpClient);
    }

    /// <summary>
    /// Creates a new OpenCodeClient instance with default options
    /// </summary>
    /// <param name="baseUrl">Base URL of OpenCode server (default: http://localhost:4096)</param>
    public OpenCodeClient(string baseUrl = "http://localhost:4096")
        : this(new OpenCodeClientOptions { BaseUrl = baseUrl })
    {
    }

    /// <summary>
    /// Creates a new OpenCodeClient instance with a custom HttpClient (recommended for Dependency Injection)
    /// </summary>
    /// <param name="httpClient">Custom HttpClient instance (should be managed by IHttpClientFactory)</param>
    /// <param name="options">Optional client configuration options</param>
    public OpenCodeClient(HttpClient httpClient, OpenCodeClientOptions? options = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _disposeHttpClient = false;
        _options = options ?? new OpenCodeClientOptions
        {
            BaseUrl = httpClient.BaseAddress?.ToString() ?? "http://localhost:4096"
        };
        _api = RestService.For<IOpenCodeApi>(_httpClient);
    }

    /// <summary>
    /// List all sessions
    /// </summary>
    /// <param name="directory">Optional directory to filter sessions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task<List<Session>> GetSessionsAsync(string? directory = null, CancellationToken cancellationToken = default)
        => _api.GetSessionsAsync(directory, cancellationToken);

    /// <summary>
    /// Create a new session
    /// </summary>
    /// <param name="title">Optional session title</param>
    /// <param name="parentId">Optional parent session ID</param>
    /// <param name="directory">Optional directory</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task<Session> CreateSessionAsync(string? title = null, string? parentId = null, string? directory = null, CancellationToken cancellationToken = default)
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
        return _api.CreateSessionAsync(request, directory, cancellationToken);
    }

    /// <summary>
    /// Get a specific session
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="directory">Optional directory</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task<Session> GetSessionAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
        => _api.GetSessionAsync(sessionId, directory, cancellationToken);

    /// <summary>
    /// Delete a session
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="directory">Optional directory</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task<bool> DeleteSessionAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
        => _api.DeleteSessionAsync(sessionId, directory, cancellationToken);

    /// <summary>
    /// Send a text prompt to a session and wait for response
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="prompt">Text prompt to send</param>
    /// <param name="providerId">Provider ID (e.g., "anthropic")</param>
    /// <param name="modelId">Model ID (e.g., "claude-3-5-sonnet-20241022")</param>
    /// <param name="directory">Optional directory</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task<PromptResponse> SendPromptAsync(
        string sessionId,
        string prompt,
        string? providerId = null,
        string? modelId = null,
        string? directory = null,
        CancellationToken cancellationToken = default)
    {
        var request = new PromptRequest
        {
            Parts = new List<MessagePart>
            {
                new MessagePart { Type = "text", Text = prompt }
            },
            Model = new ModelConfig
            {
                ProviderId = providerId ?? _options.DefaultProviderId,
                ModelId = modelId ?? _options.DefaultModelId
            }
        };
        return _api.SendPromptAsync(sessionId, request, directory, cancellationToken);
    }

    /// <summary>
    /// Send a prompt asynchronously (fire and forget)
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="prompt">Text prompt to send</param>
    /// <param name="providerId">Provider ID (e.g., "anthropic")</param>
    /// <param name="modelId">Model ID (e.g., "claude-3-5-sonnet-20241022")</param>
    /// <param name="directory">Optional directory</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task SendPromptAsyncAsync(
        string sessionId,
        string prompt,
        string? providerId = null,
        string? modelId = null,
        string? directory = null,
        CancellationToken cancellationToken = default)
    {
        var request = new PromptRequest
        {
            Parts = new List<MessagePart>
            {
                new MessagePart { Type = "text", Text = prompt }
            },
            Model = new ModelConfig
            {
                ProviderId = providerId ?? _options.DefaultProviderId,
                ModelId = modelId ?? _options.DefaultModelId
            }
        };
        return _api.SendPromptAsyncAsync(sessionId, request, directory, cancellationToken);
    }

    /// <summary>
    /// List messages for a session
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="limit">Optional limit on number of messages to return</param>
    /// <param name="directory">Optional directory</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task<List<MessageWithParts>> GetMessagesAsync(string sessionId, int? limit = null, string? directory = null, CancellationToken cancellationToken = default)
        => _api.GetMessagesAsync(sessionId, limit, directory, cancellationToken);

    /// <summary>
    /// Abort a running session
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="directory">Optional directory</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task<bool> AbortSessionAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
        => _api.AbortSessionAsync(sessionId, directory, cancellationToken);

    /// <summary>
    /// Get todos for a session
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="directory">Optional directory</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task<List<Todo>> GetTodosAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
        => _api.GetTodosAsync(sessionId, directory, cancellationToken);

    /// <summary>
    /// Create an event stream to listen for global events
    /// </summary>
    /// <returns>Event stream handler</returns>
    public OpenCodeEventStream CreateEventStream()
    {
        return new OpenCodeEventStream(_httpClient.BaseAddress?.ToString() ?? "http://localhost:4096");
    }

    // TUI API Methods

    /// <summary>
    /// Append text to the current prompt in the TUI
    /// </summary>
    /// <param name="text">Text to append</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task AppendPromptAsync(string text, CancellationToken cancellationToken = default)
        => _api.AppendPromptAsync(new AppendPromptRequest { Text = text }, cancellationToken);

    /// <summary>
    /// Submit the current prompt in the TUI
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task SubmitPromptAsync(CancellationToken cancellationToken = default)
        => _api.SubmitPromptAsync(cancellationToken);

    /// <summary>
    /// Clear the current prompt in the TUI
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task ClearPromptAsync(CancellationToken cancellationToken = default)
        => _api.ClearPromptAsync(cancellationToken);

    /// <summary>
    /// Append text to the prompt and submit it (convenience method)
    /// </summary>
    /// <param name="text">Text to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task SendToTuiAsync(string text, CancellationToken cancellationToken = default)
    {
        await _api.AppendPromptAsync(new AppendPromptRequest { Text = text }, cancellationToken);
        await Task.Delay(50, cancellationToken); // Small delay to ensure text is appended
        await _api.SubmitPromptAsync(cancellationToken);
    }

    /// <summary>
    /// Execute a command in the TUI
    /// </summary>
    /// <param name="command">Command to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task ExecuteCommandAsync(string command, CancellationToken cancellationToken = default)
        => _api.ExecuteCommandAsync(new ExecuteCommandRequest { Command = command }, cancellationToken);

    /// <summary>
    /// Show a toast notification in the TUI
    /// </summary>
    /// <param name="message">Toast message</param>
    /// <param name="type">Toast type (info, success, warning, error)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task ShowToastAsync(string message, string type = "info", CancellationToken cancellationToken = default)
        => _api.ShowToastAsync(new ShowToastRequest { Message = message, Type = type }, cancellationToken);

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
