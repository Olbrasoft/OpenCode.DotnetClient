namespace OpenCode.DotnetClient.Models;

/// <summary>
/// Represents a message in an OpenCode session
/// </summary>
public class Message
{
    /// <summary>
    /// Message ID (pattern: msg.*)
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Session ID this message belongs to
    /// </summary>
    public required string SessionId { get; init; }

    /// <summary>
    /// Message role (user, assistant, system)
    /// </summary>
    public required string Role { get; init; }

    /// <summary>
    /// Message content/text
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTimeOffset? CreatedAt { get; init; }
}
