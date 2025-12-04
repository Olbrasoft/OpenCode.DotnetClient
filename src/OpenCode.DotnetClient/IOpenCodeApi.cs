using OpenCode.DotnetClient.Models;
using Refit;

namespace OpenCode.DotnetClient;

/// <summary>
/// OpenCode API interface for Refit
/// </summary>
public interface IOpenCodeApi
{
    /// <summary>
    /// List all sessions
    /// </summary>
    [Get("/session")]
    Task<List<Session>> GetSessionsAsync([Query] string? directory = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new session
    /// </summary>
    [Post("/session")]
    Task<Session> CreateSessionAsync([Body] CreateSessionRequest request, [Query] string? directory = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a specific session
    /// </summary>
    [Get("/session/{id}")]
    Task<Session> GetSessionAsync(string id, [Query] string? directory = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a session
    /// </summary>
    [Delete("/session/{id}")]
    Task<bool> DeleteSessionAsync(string id, [Query] string? directory = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send a prompt to a session
    /// </summary>
    [Post("/session/{id}/message")]
    Task<PromptResponse> SendPromptAsync(string id, [Body] PromptRequest request, [Query] string? directory = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send a prompt asynchronously (returns immediately)
    /// </summary>
    [Post("/session/{id}/prompt_async")]
    Task SendPromptAsyncAsync(string id, [Body] PromptRequest request, [Query] string? directory = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// List messages for a session
    /// </summary>
    [Get("/session/{id}/message")]
    Task<List<MessageWithParts>> GetMessagesAsync(string id, [Query] int? limit = null, [Query] string? directory = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Abort a running session
    /// </summary>
    [Post("/session/{id}/abort")]
    Task<bool> AbortSessionAsync(string id, [Query] string? directory = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get todos for a session
    /// </summary>
    [Get("/session/{id}/todo")]
    Task<List<Todo>> GetTodosAsync(string id, [Query] string? directory = null, CancellationToken cancellationToken = default);

    // TUI API

    /// <summary>
    /// Append text to the current prompt
    /// </summary>
    [Post("/tui/append-prompt")]
    Task AppendPromptAsync([Body] AppendPromptRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Submit the current prompt
    /// </summary>
    [Post("/tui/submit-prompt")]
    Task SubmitPromptAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear the current prompt
    /// </summary>
    [Post("/tui/clear-prompt")]
    Task ClearPromptAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute a command
    /// </summary>
    [Post("/tui/execute-command")]
    Task ExecuteCommandAsync([Body] ExecuteCommandRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Show a toast notification
    /// </summary>
    [Post("/tui/show-toast")]
    Task ShowToastAsync([Body] ShowToastRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Message with its parts
/// </summary>
public class MessageWithParts
{
    /// <summary>
    /// Message information
    /// </summary>
    public required MessageInfo Info { get; init; }

    /// <summary>
    /// Message parts
    /// </summary>
    public required List<MessagePart> Parts { get; init; }
}
