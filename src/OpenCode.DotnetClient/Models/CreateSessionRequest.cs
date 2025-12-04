using System.Text.Json.Serialization;

namespace OpenCode.DotnetClient.Models;

/// <summary>
/// Request to create a new session
/// </summary>
public class CreateSessionRequest
{
    /// <summary>
    /// Parent session ID (optional)
    /// </summary>
    [JsonPropertyName("parentID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentId { get; init; }

    /// <summary>
    /// Session title (optional)
    /// </summary>
    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Title { get; init; }
}
