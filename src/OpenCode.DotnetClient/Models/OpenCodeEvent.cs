using System.Text.Json.Serialization;

namespace OpenCode.DotnetClient.Models;

/// <summary>
/// Base class for OpenCode events
/// </summary>
public class OpenCodeEvent
{
    /// <summary>
    /// Event type
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Event data (varies by type)
    /// </summary>
    [JsonPropertyName("data")]
    public object? Data { get; init; }
}

/// <summary>
/// Global event wrapper
/// </summary>
public class GlobalEvent
{
    /// <summary>
    /// Directory where the event occurred
    /// </summary>
    [JsonPropertyName("directory")]
    public required string Directory { get; init; }

    /// <summary>
    /// Event payload
    /// </summary>
    [JsonPropertyName("payload")]
    public required OpenCodeEvent Payload { get; init; }
}

/// <summary>
/// Session status event
/// </summary>
public class SessionStatusEvent
{
    /// <summary>
    /// Session ID
    /// </summary>
    [JsonPropertyName("sessionID")]
    public required string SessionId { get; init; }

    /// <summary>
    /// Session status (idle, running, etc.)
    /// </summary>
    [JsonPropertyName("status")]
    public required string Status { get; init; }
}

/// <summary>
/// Message updated event
/// </summary>
public class MessageUpdatedEvent
{
    /// <summary>
    /// Session ID
    /// </summary>
    [JsonPropertyName("sessionID")]
    public required string SessionId { get; init; }

    /// <summary>
    /// Message ID
    /// </summary>
    [JsonPropertyName("messageID")]
    public required string MessageId { get; init; }

    /// <summary>
    /// Message content
    /// </summary>
    [JsonPropertyName("message")]
    public required MessageInfo Message { get; init; }
}

/// <summary>
/// Todo updated event
/// </summary>
public class TodoUpdatedEvent
{
    /// <summary>
    /// Session ID
    /// </summary>
    [JsonPropertyName("sessionID")]
    public required string SessionId { get; init; }

    /// <summary>
    /// Updated todos
    /// </summary>
    [JsonPropertyName("todos")]
    public required List<Todo> Todos { get; init; }
}

/// <summary>
/// File edited event
/// </summary>
public class FileEditedEvent
{
    /// <summary>
    /// File path
    /// </summary>
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    /// <summary>
    /// Edit type
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }
}
