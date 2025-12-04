using System.Text.Json.Serialization;

namespace OpenCode.DotnetClient.Models;

/// <summary>
/// Represents an OpenCode session
/// </summary>
public class Session
{
    /// <summary>
    /// Session ID (pattern: ses.*)
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Session title
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// API version
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; init; }

    /// <summary>
    /// Project ID
    /// </summary>
    [JsonPropertyName("projectID")]
    public string? ProjectId { get; init; }

    /// <summary>
    /// Directory path
    /// </summary>
    [JsonPropertyName("directory")]
    public string? Directory { get; init; }

    /// <summary>
    /// Parent session ID
    /// </summary>
    [JsonPropertyName("parentID")]
    public string? ParentId { get; init; }

    /// <summary>
    /// Time information
    /// </summary>
    [JsonPropertyName("time")]
    public SessionTime? Time { get; init; }
}

/// <summary>
/// Session time information
/// </summary>
public class SessionTime
{
    /// <summary>
    /// Creation timestamp (Unix milliseconds)
    /// </summary>
    [JsonPropertyName("created")]
    public long? Created { get; init; }

    /// <summary>
    /// Last updated timestamp (Unix milliseconds)
    /// </summary>
    [JsonPropertyName("updated")]
    public long? Updated { get; init; }
}
