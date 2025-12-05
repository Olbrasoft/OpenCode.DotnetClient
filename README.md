# OpenCode.DotnetClient

[![Build](https://github.com/Olbrasoft/OpenCode.DotnetClient/actions/workflows/build.yml/badge.svg)](https://github.com/Olbrasoft/OpenCode.DotnetClient/actions/workflows/build.yml)
[![Publish NuGet](https://github.com/Olbrasoft/OpenCode.DotnetClient/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/Olbrasoft/OpenCode.DotnetClient/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/v/Olbrasoft.OpenCode.DotnetClient.svg)](https://www.nuget.org/packages/Olbrasoft.OpenCode.DotnetClient/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Olbrasoft.OpenCode.DotnetClient.svg)](https://www.nuget.org/packages/Olbrasoft.OpenCode.DotnetClient/)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

A simple .NET client library for the [OpenCode API](https://opencode.ai/), enabling easy integration of OpenCode's AI coding capabilities into C# applications.

## 📋 Overview

This is a **Proof of Concept (POC)** .NET client for OpenCode API, built with:
- **Refit** for type-safe HTTP client
- **System.Text.Json** for JSON serialization
- **xUnit** for testing

## 🚀 Features

- ✅ **Session Management**: create, get, list, delete sessions
- ✅ **Prompt Sending**: send prompts to AI models (sync + async)
- ✅ **Message Retrieval**: fetch messages from sessions
- ✅ **Todo Support**: get todo lists for sessions
- ✅ **Event Streaming**: real-time SSE event streaming with robust error handling
- ✅ **Type-Safe API**: Refit-based type-safe HTTP calls
- ✅ **Async/Await**: full async support with cancellation tokens
- ✅ **Dependency Injection**: proper HttpClient management with IHttpClientFactory support
- ✅ **Error Handling**: custom exception types for different error scenarios
- ✅ **Configuration**: flexible options for timeouts, base URL, and default model settings
- ✅ **Comprehensive Tests**: unit + integration tests
- ✅ **Production-Ready**: follows .NET best practices and clean code principles

## 📦 Installation

### NuGet Package

```bash
dotnet add package Olbrasoft.OpenCode.DotnetClient
```

Or via Package Manager Console:
```powershell
Install-Package Olbrasoft.OpenCode.DotnetClient
```

### Prerequisites

- .NET 10.0 SDK or later
- Running OpenCode server (default: `http://localhost:4096`)

### Building from Source

```bash
git clone https://github.com/Olbrasoft/OpenCode.DotnetClient.git
cd OpenCode.DotnetClient
dotnet build
```

### Running Tests

```bash
dotnet test
```

**Note:** Integration tests require a running OpenCode server at `http://localhost:4096`.

## 📖 Usage

### Basic Example

```csharp
using OpenCode.DotnetClient;

// Create client
using var client = new OpenCodeClient("http://localhost:4096");

// Create a new session
var session = await client.CreateSessionAsync("My AI Session");
Console.WriteLine($"Session created: {session.Id}");

// Send a prompt
var response = await client.SendPromptAsync(
    session.Id,
    "Write a hello world function in C#",
    providerId: "anthropic",
    modelId: "claude-3-5-sonnet-20241022"
);

// Get the response
foreach (var part in response.Parts)
{
    if (part.Type == "text")
    {
        Console.WriteLine(part.Text);
    }
}

// List messages in session
var messages = await client.GetMessagesAsync(session.Id);
Console.WriteLine($"Total messages: {messages.Count}");

// Cleanup
await client.DeleteSessionAsync(session.Id);
```

### Event Streaming

Listen to real-time events from OpenCode server:

```csharp
using var eventStream = client.CreateEventStream();

await foreach (var globalEvent in eventStream.StreamGlobalEventsAsync())
{
    Console.WriteLine($"[{globalEvent.Payload.Type}] in {globalEvent.Directory}");

    // Handle specific event types
    switch (globalEvent.Payload.Type)
    {
        case "session.status":
            Console.WriteLine("Session status changed");
            break;
        case "message.updated":
            Console.WriteLine("Message was updated");
            break;
        case "todo.updated":
            Console.WriteLine("Todo list changed");
            break;
        case "file.edited":
            Console.WriteLine("File was edited");
            break;
    }
}
```

### Todos

Get todos for a session:

```csharp
var todos = await client.GetTodosAsync(session.Id);

foreach (var todo in todos)
{
    Console.WriteLine($"[{todo.Status}] {todo.Content} (Priority: {todo.Priority})");
}
```

### Advanced Usage

#### Configuration Options

```csharp
// Create client with custom configuration
var options = new OpenCodeClientOptions
{
    BaseUrl = "http://localhost:4096",
    Timeout = TimeSpan.FromMinutes(10),
    DefaultProviderId = "anthropic",
    DefaultModelId = "claude-3-5-sonnet-20241022",
    ThrowOnError = true
};

using var client = new OpenCodeClient(options);
```

#### Dependency Injection (ASP.NET Core)

**Recommended approach** for production applications to avoid socket exhaustion:

```csharp
// In Program.cs or Startup.cs
builder.Services.AddHttpClient<OpenCodeClient>((serviceProvider, httpClient) =>
{
    httpClient.BaseAddress = new Uri("http://localhost:4096");
    httpClient.Timeout = TimeSpan.FromMinutes(5);
});

// Or with IHttpClientFactory
builder.Services.AddHttpClient("OpenCodeApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:4096");
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddScoped<OpenCodeClient>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("OpenCodeApi");
    return new OpenCodeClient(httpClient);
});

// Usage in controllers
public class MyController : ControllerBase
{
    private readonly OpenCodeClient _openCodeClient;

    public MyController(OpenCodeClient openCodeClient)
    {
        _openCodeClient = openCodeClient;
    }
}
```

#### Custom HttpClient (Simple Usage)

For console applications or simple scenarios:

```csharp
var httpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:4096"),
    Timeout = TimeSpan.FromMinutes(5)
};

using var client = new OpenCodeClient(httpClient);
```

#### Error Handling

The client includes custom exception types for better error handling:

```csharp
try
{
    var session = await client.CreateSessionAsync("My Session");
    var response = await client.SendPromptAsync(
        session.Id,
        "Write a hello world function"
    );
}
catch (OpenCodeConnectionException ex)
{
    // Connection failures (server unreachable, timeout)
    Console.WriteLine($"Connection failed: {ex.Message}");
}
catch (OpenCodeApiException ex)
{
    // API errors (4xx, 5xx responses)
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
    Console.WriteLine($"Response: {ex.ResponseContent}");
}
catch (OpenCodeException ex)
{
    // Other OpenCode errors
    Console.WriteLine($"OpenCode error: {ex.Message}");
}
```

#### Cancellation Support

All async methods support cancellation tokens:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    var sessions = await client.GetSessionsAsync(
        cancellationToken: cts.Token
    );

    var response = await client.SendPromptAsync(
        sessionId: "ses_abc123",
        prompt: "Long-running task...",
        cancellationToken: cts.Token
    );
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled");
}
```

#### Working with Sessions

```csharp
// List all sessions
var sessions = await client.GetSessionsAsync();

// Get specific session
var session = await client.GetSessionAsync("ses_abc123");

// Create session with title and parent
var childSession = await client.CreateSessionAsync(
    title: "Feature Development",
    parentId: session.Id
);

// Delete session
await client.DeleteSessionAsync(session.Id);
```

#### Sending Prompts

```csharp
// Simple text prompt
var response = await client.SendPromptAsync(
    sessionId: "ses_abc123",
    prompt: "Explain SOLID principles",
    providerId: "anthropic",
    modelId: "claude-3-5-sonnet-20241022"
);

// Async prompt (fire and forget)
await client.SendPromptAsyncAsync(
    sessionId: "ses_abc123",
    prompt: "Generate documentation"
);
```

## 🏗️ Architecture

```
OpenCode.DotnetClient/
├── src/
│   └── OpenCode.DotnetClient/
│       ├── Models/                       # DTOs for API requests/responses
│       │   ├── Session.cs
│       │   ├── Message.cs
│       │   ├── PromptRequest.cs
│       │   ├── PromptResponse.cs
│       │   ├── Todo.cs
│       │   └── OpenCodeEvent.cs          # Event models
│       ├── IOpenCodeApi.cs               # Refit API interface
│       ├── OpenCodeClient.cs             # Main client wrapper
│       ├── OpenCodeClientOptions.cs      # Configuration options
│       ├── OpenCodeException.cs          # Custom exception types
│       └── OpenCodeEventStream.cs        # SSE event streaming
├── tests/
│   └── OpenCode.DotnetClient.Tests/
│       ├── OpenCodeClientTests.cs        # Integration tests
│       └── OpenCodeClientUnitTests.cs    # Unit tests
└── examples/
    └── OpenCode.DotnetClient.Example/  # Example console app
        └── Program.cs                  # Interactive example
```

## 🔧 API Reference

### OpenCodeClient

#### Constructor
- `OpenCodeClient(string baseUrl = "http://localhost:4096")`
- `OpenCodeClient(HttpClient httpClient)`

#### Methods

**Session Management**
- `Task<List<Session>> GetSessionsAsync(string? directory = null)`
- `Task<Session> CreateSessionAsync(string? title = null, string? parentId = null, string? directory = null)`
- `Task<Session> GetSessionAsync(string sessionId, string? directory = null)`
- `Task<bool> DeleteSessionAsync(string sessionId, string? directory = null)`

**Messaging**
- `Task<PromptResponse> SendPromptAsync(string sessionId, string prompt, string providerId = "anthropic", string modelId = "claude-3-5-sonnet-20241022", string? directory = null)`
- `Task SendPromptAsyncAsync(string sessionId, string prompt, string providerId = "anthropic", string modelId = "claude-3-5-sonnet-20241022", string? directory = null)`
- `Task<List<MessageWithParts>> GetMessagesAsync(string sessionId, int? limit = null, string? directory = null)`

**Session Control**
- `Task<bool> AbortSessionAsync(string sessionId, string? directory = null)`

**Todos**
- `Task<List<Todo>> GetTodosAsync(string sessionId, string? directory = null)`

**Event Streaming**
- `OpenCodeEventStream CreateEventStream()`

## 🧪 Testing

The project includes comprehensive tests:

- **Unit Tests (4)**: Basic client functionality, constructors, disposal
- **Integration Tests (6)**: Real API calls requiring running OpenCode server

Run tests:
```bash
# All tests
dotnet test

# Only unit tests (no server required)
dotnet test --filter "FullyQualifiedName~UnitTests"

# Only integration tests
dotnet test --filter "FullyQualifiedName~OpenCodeClientTests"
```

## 🛠️ Development

### Project Structure

This is a .NET solution with two projects:
- **OpenCode.DotnetClient**: Class library (.NET 10)
- **OpenCode.DotnetClient.Tests**: Test project with xUnit

### Dependencies

- [Refit](https://github.com/reactiveui/refit) - Type-safe REST client
- [xUnit](https://xunit.net/) - Testing framework
- [Moq](https://github.com/moq/moq4) - Mocking framework

### Building

```bash
dotnet build
```

### Running OpenCode Server

Before running integration tests, start the OpenCode server:

```bash
opencode serve --port 4096
```

### Running the Example

An interactive example application is included in the `examples` directory:

```bash
cd examples/OpenCode.DotnetClient.Example
dotnet run
```

The example demonstrates:
- **Event Streaming**: Real-time monitoring of all OpenCode events
- **Interactive Sessions**: Chat with AI through the console
- **Session Management**: List and manage active sessions
- **Color-coded Output**: Beautiful terminal UI with ANSI colors

See [examples/README.md](examples/README.md) for detailed usage instructions.

## 📝 Requirements

- **.NET 10.0** or later
- **OpenCode Server** running on http://localhost:4096 (for integration tests)

## 🎯 Supported Events

The client supports real-time streaming of these OpenCode events:

### Session Events
- `session.status` - Session status changes (running, idle, etc.)
- `session.idle` - Session becomes idle

### Message Events
- `message.updated` - Message content updated
- `message.removed` - Message deleted

### Todo Events
- `todo.updated` - Todo list changes (new todos, status updates)

### File Events
- `file.edited` - File was edited by AI
- `file.watcher.updated` - File system watcher detected changes

### Other Events
- `server.instance.disposed` - Server instance cleanup
- `lsp.client.diagnostics` - LSP diagnostics from language servers
- `command.executed` - Command execution notifications
- `installation.updated` - Installation updates
- `installation.update-available` - New version available

## 🎯 Future Enhancements

Possible improvements for future versions:

- ✅ ~~Error Handling~~: **Implemented** - Custom exception types with detailed error information
- ✅ ~~Configuration~~: **Implemented** - Strongly-typed OpenCodeClientOptions
- **Retry Policies**: Add automatic retry with exponential backoff using Polly
- **NuGet Package**: Publish as reusable package to nuget.org
- **Additional Endpoints**: Support for more OpenCode API features (file operations, providers, models list, etc.)
- **CLI Tool**: Command-line interface for quick operations
- **Logging**: Integrate with ILogger for production-grade logging
- **Metrics**: Add telemetry and metrics collection
- **Connection Pooling**: Advanced HttpClient configuration for high-throughput scenarios

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Contact

For questions or issues, please open an issue on GitHub.

## 🙏 Acknowledgments

- [OpenCode](https://opencode.ai/) - The AI coding assistant
- [Refit](https://github.com/reactiveui/refit) - For the excellent HTTP client library

---

**Note:** This is a Proof of Concept (POC) implementation. For production use, additional error handling, logging, and configuration options should be added.
