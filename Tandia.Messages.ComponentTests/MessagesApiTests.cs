using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Tandia.Messages.Application.Models;
using Tandia.Messages.WebApi.DTOs.Requests;

namespace Tandia.Messages.ComponentTests;

public sealed class MessagesApiTests : IClassFixture<TandiaWebApplicationFactory>, IAsyncLifetime
{
    private readonly TandiaWebApplicationFactory factory;
    private readonly HttpClient client;

    public MessagesApiTests(TandiaWebApplicationFactory applicationFactory)
    {
        factory = applicationFactory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task SendTwoMessages_ShouldBeInRightOrder()
    {
        // Arrange
        var firstMessage = new MessageRequestDto { Content = "First" };
        var secondMessage = new MessageRequestDto { Content = "Second" };

        // Act
        await client.PutAsJsonAsync($"api/Messages/{Guid.NewGuid()}", firstMessage);
        await client.PutAsJsonAsync($"api/Messages/{Guid.NewGuid()}", secondMessage);

        // Assert
        var messages = await client.GetFromJsonAsync<List<Message>>("/api/messages");
        messages.Should().BeInAscendingOrder(m => m.Timestamp);
    }

    [Fact]
    public async Task GetMessages_WhenNoMessages_MessageListShouldBeEmpty()
    {
        // Act
        var response = await client.GetAsync("api/messages");
        var messages = await response.Content.ReadFromJsonAsync<List<Message>>();

        // Assert
        messages.Should().BeEmpty();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendMessage_WhenMessageSent_MessageAppearsInList()
    {
        // Arrange
        var newMessage = new MessageRequestDto() { Content = "text" };

        // Act
        var response = await client.PutAsJsonAsync($"api/Messages/{Guid.NewGuid()}", newMessage);
        var messages = await client.GetFromJsonAsync<List<Message>>("api/Messages");

        // Assert
        messages.Should().ContainSingle().Which.Content.Should().Be("text");
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task SendMessage_WhenSameMessageSentWithDifferentText_ShouldUpdateMessage()
    {
        // Arrange
        var text = new MessageRequestDto { Content = "Original" };
        var updatedText = new MessageRequestDto { Content = "Updated" };

        // Act
        await client.PutAsJsonAsync($"api/Messages/{Guid.NewGuid()}", text);
        var messages = await client.GetFromJsonAsync<List<Message>>("api/Messages");
        await client.PutAsJsonAsync($"api/Messages/{messages?[0].Id}", updatedText);
        var updatedMessages = await client.GetFromJsonAsync<List<Message>>("api/Messages");

        // Assert
        updatedMessages.Should().ContainSingle().Which.Content.Should().Be("Updated");
        updatedMessages[0].LastModified.Should().NotBeNull();
    }

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
