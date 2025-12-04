using System.Text.Json.Serialization;

namespace OpenCode.DotnetClient.Models;

/// <summary>
/// Represents a todo item in a session
/// </summary>
public class Todo
{
    /// <summary>
    /// Unique identifier for the todo item
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Brief description of the task
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }

    /// <summary>
    /// Current status: pending, in_progress, completed, cancelled
    /// </summary>
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    /// <summary>
    /// Priority level: high, medium, low
    /// </summary>
    [JsonPropertyName("priority")]
    public required string Priority { get; init; }
}
