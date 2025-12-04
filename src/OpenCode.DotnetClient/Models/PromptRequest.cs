using System.Text.Json.Serialization;

namespace OpenCode.DotnetClient.Models;

/// <summary>
/// Request to send a prompt to a session
/// </summary>
public class PromptRequest
{
    /// <summary>
    /// Message parts to send
    /// </summary>
    [JsonPropertyName("parts")]
    public required List<MessagePart> Parts { get; init; }

    /// <summary>
    /// Model configuration
    /// </summary>
    [JsonPropertyName("model")]
    public ModelConfig? Model { get; init; }

    /// <summary>
    /// Optional message ID
    /// </summary>
    [JsonPropertyName("messageID")]
    public string? MessageId { get; init; }

    /// <summary>
    /// Whether to skip AI reply
    /// </summary>
    [JsonPropertyName("noReply")]
    public bool? NoReply { get; init; }
}

/// <summary>
/// Model configuration for prompt
/// </summary>
public class ModelConfig
{
    /// <summary>
    /// Provider ID (e.g., "anthropic", "openai")
    /// </summary>
    [JsonPropertyName("providerID")]
    public required string ProviderId { get; init; }

    /// <summary>
    /// Model ID (e.g., "claude-3-5-sonnet-20241022")
    /// </summary>
    [JsonPropertyName("modelID")]
    public required string ModelId { get; init; }
}

/// <summary>
/// Message part (can be text, file, etc.)
/// </summary>
public class MessagePart
{
    /// <summary>
    /// Part type (text, file, etc.)
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Text content (for type="text")
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}
