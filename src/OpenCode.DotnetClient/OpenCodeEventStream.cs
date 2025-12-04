using OpenCode.DotnetClient.Models;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace OpenCode.DotnetClient;

/// <summary>
/// Handles Server-Sent Events (SSE) streaming from OpenCode server
/// </summary>
public class OpenCodeEventStream : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private CancellationTokenSource? _cts;
    private bool _disposed;

    /// <summary>
    /// Creates a new event stream handler
    /// </summary>
    public OpenCodeEventStream(string baseUrl = "http://localhost:4096")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = Timeout.InfiniteTimeSpan // SSE connections are long-lived
        };
    }

    /// <summary>
    /// Creates a new event stream handler with custom HttpClient
    /// </summary>
    public OpenCodeEventStream(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseUrl = httpClient.BaseAddress?.ToString() ?? "http://localhost:4096";
    }

    /// <summary>
    /// Streams global events from the OpenCode server
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the stream</param>
    /// <returns>Async enumerable of events</returns>
    public async IAsyncEnumerable<GlobalEvent> StreamGlobalEventsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var request = new HttpRequestMessage(HttpMethod.Get, "/global/event");
        request.Headers.Add("Accept", "text/event-stream");

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            _cts.Token);

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(_cts.Token);
        using var reader = new StreamReader(stream);

        string? line;
        string? eventData = null;

        while (!_cts.Token.IsCancellationRequested &&
               (line = await reader.ReadLineAsync(_cts.Token)) != null)
        {
            // SSE format: "data: {json}\n\n"
            if (line.StartsWith("data: "))
            {
                eventData = line.Substring(6); // Remove "data: " prefix
            }
            else if (string.IsNullOrWhiteSpace(line) && eventData != null)
            {
                // Empty line signals end of event
                GlobalEvent? globalEvent = null;
                try
                {
                    globalEvent = JsonSerializer.Deserialize<GlobalEvent>(eventData);
                }
                catch (JsonException)
                {
                    // Skip malformed events
                    eventData = null;
                    continue;
                }

                if (globalEvent != null)
                {
                    yield return globalEvent;
                }

                eventData = null;
            }
        }
    }

    /// <summary>
    /// Stops the event stream
    /// </summary>
    public void Stop()
    {
        _cts?.Cancel();
    }

    /// <summary>
    /// Disposes resources
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        _cts?.Cancel();
        _cts?.Dispose();
        _httpClient?.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
