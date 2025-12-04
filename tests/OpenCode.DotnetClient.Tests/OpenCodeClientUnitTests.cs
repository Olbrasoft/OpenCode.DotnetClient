using Xunit;

namespace OpenCode.DotnetClient.Tests;

/// <summary>
/// Unit tests for OpenCodeClient
/// </summary>
public class OpenCodeClientUnitTests
{
    [Fact]
    public void Constructor_WithDefaultUrl_ShouldCreateClient()
    {
        // Act
        using var client = new OpenCodeClient();

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithCustomUrl_ShouldCreateClient()
    {
        // Act
        using var client = new OpenCodeClient("http://example.com:8080");

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithHttpClient_ShouldCreateClient()
    {
        // Arrange
        using var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:4096") };

        // Act
        using var client = new OpenCodeClient(httpClient);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var client = new OpenCodeClient();

        // Act & Assert
        client.Dispose();
    }
}
