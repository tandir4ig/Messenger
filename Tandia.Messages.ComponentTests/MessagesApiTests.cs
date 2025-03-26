using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Tandia.Messages.Application.Models;
using Tandia.Messages.WebApi.DTOs.Requests;

namespace Tandia.Messages.ComponentTests;

public class MessagesApiTests : IClassFixture<TandiaWebApplicationFactory>, IAsyncLifetime
{
    private readonly TandiaWebApplicationFactory _factory;

    public MessagesApiTests(TandiaWebApplicationFactory applicationFactory)
    {
        _factory = applicationFactory;
    }

    [Fact]
    public async Task SendTwoMessages_ShouldBeInRightOrder()
    {
        // Arrange
        var _httpClient = _factory.CreateClient();
        var firstMessage = new MessageRequestDto { Content = "First" };
        var secondMessage = new MessageRequestDto { Content = "Second" };

        // Act
        await _httpClient.PutAsJsonAsync($"api/Messages/{Guid.NewGuid()}", firstMessage);
        await _httpClient.PutAsJsonAsync($"api/Messages/{Guid.NewGuid()}", secondMessage);

        // Assert
        var messages = await _httpClient.GetFromJsonAsync<List<Message>>("/api/messages");
        messages.Should().BeInDescendingOrder(m => m.Timestamp);
    }

    [Fact]
    public async Task GetMessages_WhenNoMessages_MessageListShouldBeEmpty()
    {
        // Arrange
        var _httpClient = _factory.CreateClient();

        // Act
        var response = await _httpClient.GetAsync("api/messages");
        var messages = await _httpClient.GetFromJsonAsync<List<Message>>("api/Messages");

        // Assert
        messages.Should().BeEmpty();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendMessage_WhenMessageSent_MessageAppearsInList()
    {
        // Arrange
        var _httpClient = _factory.CreateClient();
        var newMessage = new MessageRequestDto() { Content = "text" };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"api/Messages/{Guid.NewGuid()}", newMessage);
        var messages = await _httpClient.GetFromJsonAsync<List<Message>>("api/Messages");

        // Assert
        messages.Should().ContainSingle().Which.Content.Should().Be("text");
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task SendMessage_WhenSameMessageSentWithDifferentText_ShouldUpdateMessage()
    {
        // Arrange
        var _httpClient = _factory.CreateClient();
        var text = new MessageRequestDto { Content = "Original" };
        var updatedText = new MessageRequestDto { Content = "Updated" };

        // Act
        await _httpClient.PutAsJsonAsync($"api/Messages/{Guid.NewGuid()}", text);
        var messages = await _httpClient.GetFromJsonAsync<List<Message>>("api/Messages");
        await _httpClient.PutAsJsonAsync($"api/Messages/{messages?[0].Id}", updatedText);
        var updatedMessages = await _httpClient.GetFromJsonAsync<List<Message>>("api/Messages");

        // Assert
        updatedMessages.Should().ContainSingle().Which.Content.Should().Be("Updated");
        updatedMessages[0].LastModified.Should().NotBeNull();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
