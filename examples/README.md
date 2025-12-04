# OpenCode.DotnetClient Examples

This directory contains example applications demonstrating how to use the OpenCode.DotnetClient library.

## 🚀 OpenCode.DotnetClient.Example

An interactive console application showcasing all features of the OpenCode .NET client.

### Features

#### 1. Event Streaming Mode
- **Real-time event monitoring** from OpenCode server
- **Color-coded output** by event type:
  - 🔵 Session events (session.status, session.idle)
  - 🟢 Message events (message.updated, message.removed)
  - 🟡 Todo events (todo.updated)
  - 🟣 File events (file.edited, file.watcher.updated)
  - 🔴 Error events
  - 🔵 Other events
- **Pretty-printed JSON** for event data
- **Timestamps** for each event

#### 2. Interactive Session Mode
- **Create AI sessions** with custom titles
- **Send prompts** and receive responses
- **View message metadata** (ID, timestamp)
- **Check todos** for the session
- **Automatic cleanup** on exit

#### 3. List Sessions Mode
- **View all active sessions**
- **Session details**: ID, title, directory, creation time
- **Color-coded output** for better readability

### Running the Example

#### Prerequisites

Make sure OpenCode server is running:

```bash
opencode serve --port 4096
```

#### Run the Example

From the repository root:

```bash
cd examples/OpenCode.DotnetClient.Example
dotnet run
```

Or with custom server URL:

```bash
dotnet run http://localhost:8080
```

### Usage

When you start the example, you'll see a menu:

```
╔═══════════════════════════════════════════════╗
║                                               ║
║         OpenCode .NET Client Example         ║
║                                               ║
╚═══════════════════════════════════════════════╝

ℹ  Connecting to OpenCode server at http://localhost:4096

Choose mode:
  1 - Event Streaming (listen to all events)
  2 - Interactive Session (create session and send prompts)
  3 - List Sessions

Enter choice (1-3):
```

#### Mode 1: Event Streaming

Watch all events from the OpenCode server in real-time:

```
ℹ  Starting event stream...
ℹ  Press Ctrl+C to stop

[16:30:45.123] session.status
  {
    "sessionID": "ses_abc123",
    "status": "running"
  }

[16:30:46.456] message.updated
  {
    "sessionID": "ses_abc123",
    "messageID": "msg_xyz789",
    "message": {
      "id": "msg_xyz789",
      "role": "assistant",
      ...
    }
  }

[16:30:47.789] todo.updated
  {
    "sessionID": "ses_abc123",
    "todos": [
      {
        "id": "todo_1",
        "content": "Analyze codebase",
        "status": "in_progress",
        "priority": "high"
      }
    ]
  }
```

#### Mode 2: Interactive Session

Chat with AI through OpenCode:

```
ℹ  Creating new session...
✓  Session created: ses_abc123

Interactive Session
Type 'exit' to quit, 'todos' to show todos

You > Hello! Can you help me with C#?

ℹ  Sending prompt...

AI >
Of course! I'd be happy to help you with C#. What specific aspect of C# would you like assistance with?

ℹ  Message ID: msg_xyz789
ℹ  Created: 2025-12-04 16:30:45

You > todos

Todos:
  🟡 [in_progress] Analyze user's C# question
  🟢 [pending] Prepare helpful response

You > exit

ℹ  Cleaning up session...
✓  Session deleted
```

#### Mode 3: List Sessions

View all active sessions:

```
ℹ  Fetching sessions...

Active Sessions:

  ses_abc123
    Title: Example Session - 2025-12-04 16:30:00
    Directory: /home/user/project
    Created: 2025-12-04 16:30:00

  ses_def456
    Title: Another Session
    Directory: /home/user/other
    Created: 2025-12-04 15:15:30

✓  Total: 2 session(s)
```

### Event Types

The example handles these OpenCode event types:

- **Session Events**
  - `session.status` - Session status changes
  - `session.idle` - Session becomes idle

- **Message Events**
  - `message.updated` - Message content updated
  - `message.removed` - Message deleted

- **Todo Events**
  - `todo.updated` - Todo list changes

- **File Events**
  - `file.edited` - File was edited
  - `file.watcher.updated` - File watcher detected changes

- **Other Events**
  - `server.instance.disposed` - Server instance cleanup
  - `lsp.client.diagnostics` - LSP diagnostics
  - `command.executed` - Command execution

### Color Codes

The example uses ANSI color codes for terminal output:

- 🔵 **Blue** - Info messages, session events
- 🟢 **Green** - Success messages, message events
- 🟡 **Yellow** - Warnings, todo events
- 🟣 **Magenta** - File events
- 🔴 **Red** - Errors
- 🔵 **Cyan** - Highlights, other events
- ⚪ **Gray** - Timestamps, metadata

### Building

```bash
dotnet build
```

### Code Structure

- `Program.cs` - Main application with three modes
  - `RunEventStreamingAsync` - SSE event streaming
  - `RunInteractiveSessionAsync` - Interactive chat
  - `ListSessionsAsync` - Session listing
  - `ShowTodosAsync` - Todo display
  - `PrintEvent` - Event formatting
  - Helper methods for colored output

## 📝 Notes

- The example requires .NET 10.0 or later
- Colors may not display correctly on all terminals
- Event streaming runs indefinitely until Ctrl+C
- Interactive mode automatically cleans up the session on exit
- The example uses the default Anthropic Claude model for AI responses

## 🔧 Troubleshooting

**"Failed to connect to OpenCode server"**
- Make sure OpenCode server is running: `opencode serve --port 4096`
- Check the server URL is correct
- Verify firewall settings

**"Your credit balance is too low"**
- This error comes from the AI provider (Anthropic)
- Configure a valid API key with credits
- The client itself is working correctly

**Colors not showing**
- Your terminal may not support ANSI colors
- Try a different terminal emulator
- On Windows, use Windows Terminal or ConEmu

## 📚 Further Reading

- [OpenCode Documentation](https://opencode.ai/docs)
- [OpenCode .NET Client README](../../README.md)
- [Refit Documentation](https://github.com/reactiveui/refit)
