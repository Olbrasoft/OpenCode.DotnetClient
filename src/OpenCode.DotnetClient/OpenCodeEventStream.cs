using OpenCode.DotnetClient.Models;
using System.Net;
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
    private readonly bool _disposeHttpClient;
    private CancellationTokenSource? _cts;
    private bool _disposed;

    /// <summary>
    /// Creates a new event stream handler
    /// </summary>
    /// <param name="baseUrl">Base URL of the OpenCode server</param>
    public OpenCodeEventStream(string baseUrl = "http://localhost:4096")
    {
        _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = Timeout.InfiniteTimeSpan // SSE connections are long-lived
        };
        _disposeHttpClient = true;
    }

    /// <summary>
    /// Creates a new event stream handler with custom HttpClient
    /// </summary>
    /// <param name="httpClient">Custom HttpClient instance</param>
    public OpenCodeEventStream(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _baseUrl = httpClient.BaseAddress?.ToString() ?? "http://localhost:4096";
        _disposeHttpClient = false;
    }

    /// <summary>
    /// Streams global events from the OpenCode server
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the stream</param>
    /// <returns>Async enumerable of events</returns>
    /// <exception cref="OpenCodeConnectionException">Thrown when connection to server fails</exception>
    /// <exception cref="OpenCodeApiException">Thrown when server returns an error response</exception>
    public async IAsyncEnumerable<GlobalEvent> StreamGlobalEventsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(OpenCodeEventStream));
        }

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var request = new HttpRequestMessage(HttpMethod.Get, "/global/event");
        request.Headers.Add("Accept", "text/event-stream");

        HttpResponseMessage? response = null;
        try
        {
            response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                _cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(_cts.Token);
                throw new OpenCodeApiException(
                    $"Failed to connect to event stream: {response.StatusCode}",
                    response.StatusCode,
                    errorContent);
            }
        }
        catch (HttpRequestException ex)
        {
            throw new OpenCodeConnectionException(
                $"Failed to connect to OpenCode server at {_baseUrl}: {ex.Message}",
                ex);
        }
        catch (TaskCanceledException ex) when (!_cts.Token.IsCancellationRequested)
        {
            throw new OpenCodeConnectionException(
                $"Connection to OpenCode server timed out: {ex.Message}",
                ex);
        }

        using (response)
        {
            using var stream = await response.Content.ReadAsStreamAsync(_cts.Token);
            using var reader = new StreamReader(stream);

            string? line;
            string? eventData = null;

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    line = await reader.ReadLineAsync(_cts.Token);
                    if (line == null) break; // End of stream
                }
                catch (OperationCanceledException)
                {
                    yield break; // Graceful cancellation
                }
                catch (IOException ex)
                {
                    throw new OpenCodeConnectionException(
                        $"Connection to OpenCode server lost: {ex.Message}",
                        ex);
                }

                // SSE format: "data: {json}\n\n"
                if (line.StartsWith("data: "))
                {
                    eventData = line.Substring(6); // Remove "data: " prefix

                    // Handle special SSE signals
                    if (eventData == "[DONE]")
                    {
                        yield break;
                    }
                }
                else if (string.IsNullOrWhiteSpace(line) && eventData != null)
                {
                    // Empty line signals end of event
                    GlobalEvent? globalEvent = null;
                    try
                    {
                        globalEvent = JsonSerializer.Deserialize<GlobalEvent>(eventData);
                    }
                    catch (JsonException ex)
                    {
                        // Log malformed event but continue streaming
                        // In production, this could use ILogger
                        Console.Error.WriteLine($"Failed to deserialize event: {ex.Message}");
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

        if (_disposeHttpClient)
        {
            _httpClient?.Dispose();
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
