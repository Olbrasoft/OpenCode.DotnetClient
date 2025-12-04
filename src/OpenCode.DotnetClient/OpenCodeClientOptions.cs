namespace OpenCode.DotnetClient;

/// <summary>
/// Configuration options for OpenCodeClient
/// </summary>
public class OpenCodeClientOptions
{
    /// <summary>
    /// Base URL of the OpenCode server
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:4096";

    /// <summary>
    /// Timeout for HTTP requests
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Default provider ID for AI prompts
    /// </summary>
    public string DefaultProviderId { get; set; } = "anthropic";

    /// <summary>
    /// Default model ID for AI prompts
    /// </summary>
    public string DefaultModelId { get; set; } = "claude-3-5-sonnet-20241022";

    /// <summary>
    /// Whether to throw exceptions on HTTP errors or return null/default values
    /// </summary>
    public bool ThrowOnError { get; set; } = true;
}
