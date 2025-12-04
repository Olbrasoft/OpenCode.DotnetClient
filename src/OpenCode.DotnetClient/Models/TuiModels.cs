using System.Text.Json.Serialization;

namespace OpenCode.DotnetClient.Models;

/// <summary>
/// Request to append text to the current prompt
/// </summary>
public class AppendPromptRequest
{
    /// <summary>
    /// Text to append to the prompt
    /// </summary>
    [JsonPropertyName("text")]
    public required string Text { get; init; }
}

/// <summary>
/// Request to execute a command
/// </summary>
public class ExecuteCommandRequest
{
    /// <summary>
    /// Command to execute
    /// </summary>
    [JsonPropertyName("command")]
    public required string Command { get; init; }
}

/// <summary>
/// Request to show a toast notification
/// </summary>
public class ShowToastRequest
{
    /// <summary>
    /// Toast message
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    /// <summary>
    /// Toast type (info, success, warning, error)
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "info";
}
