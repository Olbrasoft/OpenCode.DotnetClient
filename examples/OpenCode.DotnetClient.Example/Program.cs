using OpenCode.DotnetClient;
using OpenCode.DotnetClient.Models;
using System.Text.Json;

namespace OpenCode.DotnetClient.Example;

/// <summary>
/// Example console application demonstrating OpenCode.DotnetClient usage
/// </summary>
class Program
{
    // ANSI color codes for terminal output
    private const string ColorReset = "\x1b[0m";
    private const string ColorRed = "\x1b[91m";
    private const string ColorGreen = "\x1b[92m";
    private const string ColorYellow = "\x1b[93m";
    private const string ColorBlue = "\x1b[94m";
    private const string ColorMagenta = "\x1b[95m";
    private const string ColorCyan = "\x1b[96m";
    private const string ColorGray = "\x1b[90m";
    private const string ColorWhite = "\x1b[97m";
    private const string Bold = "\x1b[1m";

    static async Task Main(string[] args)
    {
        var serverUrl = args.Length > 0 ? args[0] : "http://localhost:4096";

        PrintHeader();
        LogInfo($"Connecting to OpenCode server at {ColorCyan}{serverUrl}{ColorReset}");

        try
        {
            // Choose mode
            Console.WriteLine();
            Console.WriteLine($"{Bold}Choose mode:{ColorReset}");
            Console.WriteLine($"  {ColorGreen}1{ColorReset} - Event Streaming (listen to all events)");
            Console.WriteLine($"  {ColorGreen}2{ColorReset} - Interactive Session (create session and send prompts)");
            Console.WriteLine($"  {ColorGreen}3{ColorReset} - List Sessions");
            Console.Write($"\n{Bold}Enter choice (1-3): {ColorReset}");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await RunEventStreamingAsync(serverUrl);
                    break;
                case "2":
                    await RunInteractiveSessionAsync(serverUrl);
                    break;
                case "3":
                    await ListSessionsAsync(serverUrl);
                    break;
                default:
                    LogError("Invalid choice. Please run again and choose 1, 2, or 3.");
                    break;
            }
        }
        catch (HttpRequestException ex)
        {
            LogError($"Failed to connect to OpenCode server: {ex.Message}");
            LogInfo($"Make sure OpenCode server is running: {ColorCyan}opencode serve --port 4096{ColorReset}");
        }
        catch (Exception ex)
        {
            LogError($"Unexpected error: {ex.Message}");
        }
    }

    static async Task RunEventStreamingAsync(string serverUrl)
    {
        LogInfo("Starting event stream...");
        LogInfo($"Press {ColorYellow}Ctrl+C{ColorReset} to stop");
        Console.WriteLine();

        using var eventStream = new OpenCodeEventStream(serverUrl);

        try
        {
            await foreach (var globalEvent in eventStream.StreamGlobalEventsAsync())
            {
                PrintEvent(globalEvent);
            }
        }
        catch (OperationCanceledException)
        {
            LogInfo("Event stream stopped");
        }
    }

    static async Task RunInteractiveSessionAsync(string serverUrl)
    {
        using var client = new OpenCodeClient(serverUrl);

        LogInfo("Creating new session...");
        var session = await client.CreateSessionAsync($"Example Session - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        LogSuccess($"Session created: {ColorCyan}{session.Id}{ColorReset}");

        Console.WriteLine();
        Console.WriteLine($"{Bold}Interactive Session{ColorReset}");
        Console.WriteLine($"Type {ColorYellow}'exit'{ColorReset} to quit, {ColorYellow}'todos'{ColorReset} to show todos");
        Console.WriteLine();

        while (true)
        {
            Console.Write($"{ColorGreen}You{ColorReset} > ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.ToLower() == "exit")
                break;

            if (input.ToLower() == "todos")
            {
                await ShowTodosAsync(client, session.Id);
                continue;
            }

            try
            {
                LogInfo("Sending prompt...");

                var response = await client.SendPromptAsync(
                    session.Id,
                    input,
                    "anthropic",
                    "claude-3-5-sonnet-20241022"
                );

                Console.WriteLine($"\n{ColorBlue}AI{ColorReset} > ");

                foreach (var part in response.Parts)
                {
                    if (part.Type == "text" && !string.IsNullOrEmpty(part.Text))
                    {
                        Console.WriteLine(part.Text);
                    }
                }

                // Show message info
                Console.WriteLine();
                LogInfo($"Message ID: {ColorGray}{response.Info.Id}{ColorReset}");
                if (response.Info.TimeCreated.HasValue)
                {
                    LogInfo($"Created: {ColorGray}{response.Info.TimeCreated:yyyy-MM-dd HH:mm:ss}{ColorReset}");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                LogError($"Failed to send prompt: {ex.Message}");
            }
        }

        LogInfo("Cleaning up session...");
        await client.DeleteSessionAsync(session.Id);
        LogSuccess("Session deleted");
    }

    static async Task ListSessionsAsync(string serverUrl)
    {
        using var client = new OpenCodeClient(serverUrl);

        LogInfo("Fetching sessions...");
        var sessions = await client.GetSessionsAsync();

        if (sessions.Count == 0)
        {
            LogInfo("No sessions found");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"{Bold}Active Sessions:{ColorReset}");
        Console.WriteLine();

        foreach (var session in sessions)
        {
            Console.WriteLine($"  {ColorCyan}{session.Id}{ColorReset}");
            Console.WriteLine($"    Title: {ColorWhite}{session.Title ?? "(no title)"}{ColorReset}");
            Console.WriteLine($"    Directory: {ColorGray}{session.Directory ?? "(none)"}{ColorReset}");
            if (session.Time?.Created != null)
            {
                var created = DateTimeOffset.FromUnixTimeMilliseconds(session.Time.Created.Value);
                Console.WriteLine($"    Created: {ColorGray}{created:yyyy-MM-dd HH:mm:ss}{ColorReset}");
            }
            Console.WriteLine();
        }

        LogSuccess($"Total: {sessions.Count} session(s)");
    }

    static async Task ShowTodosAsync(OpenCodeClient client, string sessionId)
    {
        try
        {
            var todos = await client.GetTodosAsync(sessionId);

            if (todos.Count == 0)
            {
                LogInfo("No todos for this session");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"{Bold}Todos:{ColorReset}");

            foreach (var todo in todos)
            {
                var statusColor = todo.Status switch
                {
                    "completed" => ColorGreen,
                    "in_progress" => ColorYellow,
                    "pending" => ColorGray,
                    "cancelled" => ColorRed,
                    _ => ColorWhite
                };

                var priorityIcon = todo.Priority switch
                {
                    "high" => "🔴",
                    "medium" => "🟡",
                    "low" => "🟢",
                    _ => "⚪"
                };

                Console.WriteLine($"  {priorityIcon} [{statusColor}{todo.Status}{ColorReset}] {todo.Content}");
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            LogError($"Failed to fetch todos: {ex.Message}");
        }
    }

    static void PrintEvent(GlobalEvent globalEvent)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var eventType = globalEvent.Payload.Type;

        // Color-code by event type
        var color = eventType switch
        {
            _ when eventType.Contains("session") => ColorBlue,
            _ when eventType.Contains("message") => ColorGreen,
            _ when eventType.Contains("todo") => ColorYellow,
            _ when eventType.Contains("file") => ColorMagenta,
            _ when eventType.Contains("error") => ColorRed,
            _ => ColorCyan
        };

        Console.WriteLine($"{ColorGray}[{timestamp}]{ColorReset} {color}{eventType}{ColorReset}");

        // Pretty print the event data
        if (globalEvent.Payload.Data != null)
        {
            try
            {
                var json = JsonSerializer.Serialize(globalEvent.Payload.Data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Indent the JSON
                foreach (var line in json.Split('\n'))
                {
                    Console.WriteLine($"  {ColorGray}{line}{ColorReset}");
                }
            }
            catch
            {
                Console.WriteLine($"  {ColorGray}{globalEvent.Payload.Data}{ColorReset}");
            }
        }

        Console.WriteLine();
    }

    static void PrintHeader()
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine($"{Bold}{ColorCyan}╔═══════════════════════════════════════════════╗{ColorReset}");
        Console.WriteLine($"{Bold}{ColorCyan}║                                               ║{ColorReset}");
        Console.WriteLine($"{Bold}{ColorCyan}║         {ColorWhite}OpenCode .NET Client Example{ColorCyan}        ║{ColorReset}");
        Console.WriteLine($"{Bold}{ColorCyan}║                                               ║{ColorReset}");
        Console.WriteLine($"{Bold}{ColorCyan}╚═══════════════════════════════════════════════╝{ColorReset}");
        Console.WriteLine();
    }

    static void LogInfo(string message)
    {
        Console.WriteLine($"{ColorBlue}ℹ{ColorReset}  {message}");
    }

    static void LogSuccess(string message)
    {
        Console.WriteLine($"{ColorGreen}✓{ColorReset}  {message}");
    }

    static void LogError(string message)
    {
        Console.WriteLine($"{ColorRed}✗{ColorReset}  {message}");
    }
}
