# OpenCode.DotnetClient

[![.NET](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

A simple .NET client library for the [OpenCode API](https://opencode.ai/), enabling easy integration of OpenCode's AI coding capabilities into C# applications.

## 📋 Overview

This is a **Proof of Concept (POC)** .NET client for OpenCode API, built with:
- **Refit** for type-safe HTTP client
- **System.Text.Json** for JSON serialization
- **xUnit** for testing

## 🚀 Features

- ✅ Session management (create, get, list, delete)
- ✅ Send prompts to AI models
- ✅ Retrieve messages from sessions
- ✅ Async/await support
- ✅ Type-safe API calls with Refit
- ✅ Comprehensive unit and integration tests

## 📦 Installation

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

### Advanced Usage

#### Custom HttpClient

```csharp
var httpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:4096"),
    Timeout = TimeSpan.FromMinutes(5)
};

using var client = new OpenCodeClient(httpClient);
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
│       ├── Models/               # DTOs for API requests/responses
│       │   ├── Session.cs
│       │   ├── Message.cs
│       │   ├── PromptRequest.cs
│       │   └── PromptResponse.cs
│       ├── IOpenCodeApi.cs       # Refit API interface
│       └── OpenCodeClient.cs     # Main client wrapper
└── tests/
    └── OpenCode.DotnetClient.Tests/
        ├── OpenCodeClientTests.cs      # Integration tests
        └── OpenCodeClientUnitTests.cs  # Unit tests
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

## 📝 Requirements

- **.NET 10.0** or later
- **OpenCode Server** running on http://localhost:4096 (for integration tests)

## 🎯 Future Enhancements

Possible improvements for a production-ready version:

- **Streaming Support**: SSE for real-time responses
- **Events System**: Observable events for session updates
- **Error Handling**: Retry policies with Polly
- **Configuration**: Strongly-typed configuration options
- **NuGet Package**: Publish as reusable package
- **Additional Endpoints**: Support for more OpenCode API features
- **CLI Tool**: Command-line interface for quick operations

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
