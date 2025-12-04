using OpenCode.DotnetClient.Models;
using Xunit;

namespace OpenCode.DotnetClient.Tests;

/// <summary>
/// Integration tests for OpenCodeClient
/// These tests require a running OpenCode server at http://localhost:4096
/// </summary>
public class OpenCodeClientTests : IDisposable
{
    private readonly OpenCodeClient _client;

    public OpenCodeClientTests()
    {
        _client = new OpenCodeClient("http://localhost:4096");
    }

    [Fact]
    public async Task CreateSession_ShouldReturnSession()
    {
        // Act
        var session = await _client.CreateSessionAsync("Test Session");

        // Assert
        Assert.NotNull(session);
        Assert.NotNull(session.Id);
        Assert.StartsWith("ses", session.Id);
        Assert.Equal("Test Session", session.Title);

        // Cleanup
        await _client.DeleteSessionAsync(session.Id);
    }

    [Fact]
    public async Task GetSessions_ShouldReturnListOfSessions()
    {
        // Arrange
        var session = await _client.CreateSessionAsync("Test Session");

        // Act
        var sessions = await _client.GetSessionsAsync();

        // Assert
        Assert.NotNull(sessions);
        Assert.Contains(sessions, s => s.Id == session.Id);

        // Cleanup
        await _client.DeleteSessionAsync(session.Id);
    }

    [Fact]
    public async Task GetSession_ShouldReturnSpecificSession()
    {
        // Arrange
        var createdSession = await _client.CreateSessionAsync("Test Session");

        // Act
        var session = await _client.GetSessionAsync(createdSession.Id);

        // Assert
        Assert.NotNull(session);
        Assert.Equal(createdSession.Id, session.Id);
        Assert.Equal("Test Session", session.Title);

        // Cleanup
        await _client.DeleteSessionAsync(session.Id);
    }

    [Fact]
    public async Task DeleteSession_ShouldReturnTrue()
    {
        // Arrange
        var session = await _client.CreateSessionAsync("Test Session");

        // Act
        var result = await _client.DeleteSessionAsync(session.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SendPrompt_ShouldReturnResponse()
    {
        // Arrange
        var session = await _client.CreateSessionAsync("Test Session");

        // Act
        var response = await _client.SendPromptAsync(
            session.Id,
            "Say 'Hello from OpenCode.DotnetClient POC'",
            "anthropic",
            "claude-3-5-sonnet-20241022"
        );

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Info);
        Assert.Equal(session.Id, response.Info.SessionId);
        Assert.NotNull(response.Parts);
        Assert.NotEmpty(response.Parts);

        // Cleanup
        await _client.DeleteSessionAsync(session.Id);
    }

    [Fact]
    public async Task GetMessages_ShouldReturnMessages()
    {
        // Arrange
        var session = await _client.CreateSessionAsync("Test Session");
        await _client.SendPromptAsync(
            session.Id,
            "Test message",
            "anthropic",
            "claude-3-5-sonnet-20241022"
        );

        // Act
        var messages = await _client.GetMessagesAsync(session.Id);

        // Assert
        Assert.NotNull(messages);
        Assert.NotEmpty(messages);

        // Cleanup
        await _client.DeleteSessionAsync(session.Id);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}
