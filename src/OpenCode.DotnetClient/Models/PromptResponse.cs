using System.Text.Json.Serialization;

namespace OpenCode.DotnetClient.Models;

/// <summary>
/// Response from sending a prompt
/// </summary>
public class PromptResponse
{
    /// <summary>
    /// Message information
    /// </summary>
    [JsonPropertyName("info")]
    public required MessageInfo Info { get; init; }

    /// <summary>
    /// Message parts
    /// </summary>
    [JsonPropertyName("parts")]
    public required List<MessagePart> Parts { get; init; }
}

/// <summary>
/// Message metadata
/// </summary>
public class MessageInfo
{
    /// <summary>
    /// Message ID
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Session ID
    /// </summary>
    [JsonPropertyName("sessionID")]
    public required string SessionId { get; init; }

    /// <summary>
    /// Message role
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    [JsonPropertyName("timeCreated")]
    public DateTimeOffset? TimeCreated { get; init; }
}
