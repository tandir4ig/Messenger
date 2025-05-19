using Blazored.LocalStorage;
using MessageClient;
using MessageClient.HttpClients;
using MessageClient.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var identityBase = new Uri(builder.Configuration
    .GetConnectionString("IdentityApi") ?? throw new InvalidOperationException("Connection string 'IdentityApi' not found."));
var messagesBase = new Uri(builder.Configuration
    .GetConnectionString("MessagesApi") ?? throw new InvalidOperationException("Connection string 'MessagesApi' not found."));

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthorizedHandler>();
builder.Services.AddScoped<ITokenStorageService, LocalStorageTokenService>();

builder.Services
    .AddHttpClient<IdentityApiClient>(c => c.BaseAddress = identityBase);

var messageClient = builder.Services.AddHttpClient<MessagesApiClient>(c => c.BaseAddress = messagesBase);
messageClient.AddStandardResilienceHandler();
messageClient.AddHttpMessageHandler<AuthorizedHandler>();

await builder.Build().RunAsync();
