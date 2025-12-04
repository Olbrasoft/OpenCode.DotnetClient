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
    Task<List<Session>> GetSessionsAsync([Query] string? directory = null);

    /// <summary>
    /// Create a new session
    /// </summary>
    [Post("/session")]
    Task<Session> CreateSessionAsync([Body] CreateSessionRequest request, [Query] string? directory = null);

    /// <summary>
    /// Get a specific session
    /// </summary>
    [Get("/session/{id}")]
    Task<Session> GetSessionAsync(string id, [Query] string? directory = null);

    /// <summary>
    /// Delete a session
    /// </summary>
    [Delete("/session/{id}")]
    Task<bool> DeleteSessionAsync(string id, [Query] string? directory = null);

    /// <summary>
    /// Send a prompt to a session
    /// </summary>
    [Post("/session/{id}/message")]
    Task<PromptResponse> SendPromptAsync(string id, [Body] PromptRequest request, [Query] string? directory = null);

    /// <summary>
    /// Send a prompt asynchronously (returns immediately)
    /// </summary>
    [Post("/session/{id}/prompt_async")]
    Task SendPromptAsyncAsync(string id, [Body] PromptRequest request, [Query] string? directory = null);

    /// <summary>
    /// List messages for a session
    /// </summary>
    [Get("/session/{id}/message")]
    Task<List<MessageWithParts>> GetMessagesAsync(string id, [Query] int? limit = null, [Query] string? directory = null);

    /// <summary>
    /// Abort a running session
    /// </summary>
    [Post("/session/{id}/abort")]
    Task<bool> AbortSessionAsync(string id, [Query] string? directory = null);

    /// <summary>
    /// Get todos for a session
    /// </summary>
    [Get("/session/{id}/todo")]
    Task<List<Todo>> GetTodosAsync(string id, [Query] string? directory = null);
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
