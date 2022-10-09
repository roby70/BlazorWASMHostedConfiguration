using BlazorApp.Client;
using BlazorApp.Shared.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.JSInterop;
using System.Diagnostics;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Here startup code of WebAssemblyHostBuilder
// https://source.dot.net/#Microsoft.AspNetCore.Components.WebAssembly/Hosting/WebAssemblyHostBuilder.cs,cb05aad758ee3460
// This is the class used to retrieve settings from the server (in 
// https://source.dot.net/#Microsoft.AspNetCore.Components.WebAssembly/Services/DefaultWebAssemblyJSRuntime.cs
var customProvider = builder.Services.BuildServiceProvider();
using (var httpClient = new HttpClient()) {
    httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    var appSettingsJson = await httpClient.GetStreamAsync("/appsettings.server.json");
    if (appSettingsJson != null) {
        builder.Configuration.Add<JsonStreamConfigurationSource>(s =>
            s.Stream = appSettingsJson);
    }
}

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// allow the usage of IOptions<MyClientSettings> in dependency Injection
builder.Services.Configure<MyClientSettings>(
    builder.Configuration.GetSection("MySettings"));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
