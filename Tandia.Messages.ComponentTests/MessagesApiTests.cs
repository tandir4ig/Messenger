using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Tandia.Messages.Application.Models;

namespace Tandia.Messages.ComponentTests;

public class MessagesApiTests : IClassFixture<TandiaWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private readonly TandiaWebApplicationFactory _factory;

    public MessagesApiTests(TandiaWebApplicationFactory applicationFactory)
    {
        _httpClient = applicationFactory.CreateClient();
        _factory = applicationFactory;
    }

    [Fact]
    public async Task GetMessages_InitiallyEmpty()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/messages");
        var messages = await _httpClient.GetFromJsonAsync<List<Message>>("api/Messages");

        // Assert
        messages.Should().BeEmpty();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    //[Fact]
    //public async Task SendMessage_MessageAppearsInList()
    //{
    //    var newMessage = new Message { Content = "Hello, World!" };
    //    var response = await _client.PostAsJsonAsync("/api/messages", newMessage);
    //    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

    //    var getResponse = await _client.GetAsync("/api/messages");
    //    var messages = await getResponse.Content.ReadAsAsync<List<Message>>();
    //    messages.Should().ContainSingle().Which.Content.Should().Be("Hello, World!");
    //}

    //[Fact]
    //public async Task SendTwoMessages_MessagesInOrder()
    //{
    //    var firstMessage = new Message { Content = "First" };
    //    var secondMessage = new Message { Content = "Second" };

    //    await _client.PostAsJsonAsync("/api/messages", firstMessage);
    //    await _client.PostAsJsonAsync("/api/messages", secondMessage);

    //    var response = await _client.GetAsync("/api/messages");
    //    var messages = await response.Content.ReadAsAsync<List<Message>>();

    //    messages.Should().HaveCount(2);
    //    messages[0].Content.Should().Be("First");
    //    messages[1].Content.Should().Be("Second");
    //}

    //[Fact]
    //public async Task UpdateMessage_MessageUpdatedInList()
    //{
    //    var message = new Message { Content = "Original" };
    //    var postResponse = await _client.PostAsJsonAsync("/api/messages", message);
    //    var createdMessage = await postResponse.Content.ReadAsAsync<Message>();

    //    createdMessage.Content = "Updated";
    //    await _client.PutAsJsonAsync($"/api/messages/{createdMessage.Id}", createdMessage);

    //    var response = await _client.GetAsync("/api/messages");
    //    var messages = await response.Content.ReadAsAsync<List<Message>>();

    //    messages.Should().ContainSingle().Which.Content.Should().Be("Updated");
    //}
}
