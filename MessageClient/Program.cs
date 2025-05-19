using Blazored.LocalStorage;
using MessageClient;
using MessageClient.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var identityBase = new Uri("https://localhost:7166/");
var messagesBase = new Uri("https://localhost:7238/");

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthorizedHandler>();
builder.Services.AddScoped<ITokenStorageService, LocalStorageTokenService>();

builder.Services.AddHttpClient("IdentityApi", c => c.BaseAddress = identityBase);

var messageClient = builder.Services.AddHttpClient("MessagesApi", client => client.BaseAddress = messagesBase);
messageClient.AddStandardResilienceHandler();
messageClient.AddHttpMessageHandler<AuthorizedHandler>();

builder.Services.AddScoped(
    sp => sp.GetRequiredService<IHttpClientFactory>()
            .CreateClient("MessagesApi"));

await builder.Build().RunAsync();
