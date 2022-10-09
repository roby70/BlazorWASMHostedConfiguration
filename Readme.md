# Manage Blazor WASM configuration

In this repository you'll find a sample about managing the configuration of a Blazor WASM application providing settings from the server side.

## Why getting configuration from the host server

The use case was an application organized by microservices based on Docker containers. In the specific case in the Blazor WASM app there are some settings like the uri of the OpenId Connect identity provider that can change between various environments. 

With docker, every ASP.NET app that run in a container could be configured, in addition to the appsetings.json file present in the project, by environment variables defined in the image. 

I thinked that would be nice if I can use the same approach to configure the Blazor app in the same way.

## Setup of Blazor App to use settings

In the example code a class `MySettings.cs` is defined with a `HelloMessage` property.

I choosed to use dependency injection using the .NET method that require  `IOptions<MyClientSettings>` service. In the Index page of the sample application this is the code that retrieve the settings.

```CSharp
...
	[Inject] 
	public IOptions<MyClientSettings> MyClientSettingOptions 
		{ get; set; } = null!;

    MyClientSettings Settings = new MyClientSettings();

    protected override async Task OnInitializedAsync() {
        Settings = MyClientSettingOptions.Value;
    }
...
```

To use this type of configuration I needed to add [Microsoft.Extensions.Options.ConfigurationExtensions](https://www.nuget.org/packages/Microsoft.Extensions.Options.ConfigurationExtensions) package as described in [Blazor ASP.NET Core configurazione | Microsoft Learn](https://learn.microsoft.com/it-it/aspnet/core/blazor/fundamentals/configuration?view=aspnetcore-6.0). 

This is the code added in the Program.cs file to manage this kind of setting:

```CSharp
...
builder.Services.Configure<MyClientSettings>(
    builder.Configuration.GetSection("MySettings"));
...
```

In the `wwwroot` folder of BlazorApp.Client there is a `appsettings.json` file where the HelloMessage is set to "Hello, world from BlazorApp.Client configuration file!". 

```json
{
  "MySettings": {
    "HelloMessage": "Hello, world from BlazorApp.Client configuration file!"
  }
}
```

If you try to run directly the BlazorApp.Client from Visual Studio you will find that the previous message will be shown.

## Setup host server to provide appsettings.json file

The basic idea is to insert a configuration part in the server `appsetting.json` file that could integrate/overwrite client side configuration. 

In the specific case I would like to overwrite the `HelloMessage` setting of `MySettings` part. 
To process only settings related to client side application I've used a top level setting named `ClientSettings` in server side `appsettings.json`.

```json
{
  ...
  "ClientSettings": {
    "MySettings": {
      "HelloMessage": "Hello, world from BlazorApp.Server configuration file!"
    }
  }
}

```



## Add settings from server to client configuration

In client startup a file (named  `appsettings.server.json`) retrieved from the server was added to the Configuration. To retrieve the file the HttpClient class could be used.

```Csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);

var customProvider = builder.Services.BuildServiceProvider();
using (var httpClient = new HttpClient()) {
    httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    var appSettingsJson = await httpClient.GetStreamAsync("/appsettings.server.json");
    if (appSettingsJson != null) {
        builder.Configuration.Add<JsonStreamConfigurationSource>(s =>
            s.Stream = appSettingsJson);
    }
}
```

The code is placed after `WebAssemblyHostBuilder.CreateDefault` where the default appsettings.json file was retrieved from the server (you could check  [WebAssemblyHostBuilder source code]([WebAssemblyHostBuilder.cs (dot.net)](https://source.dot.net/#Microsoft.AspNetCore.Components.WebAssembly/Hosting/WebAssemblyHostBuilder.cs,cb05aad758ee3460)) to check for the startup process)

> In WebAssemblyHostBuilder the IJSUnmarshalledRuntime service is used to retrieve the appsettings.json file. But the class that implements this interface is not public, so HttpClient was used to retrieve the other file.

In the server the *minimal API* routing was used to retrieve the file, 

```csharp
// Place this as first to avoid Blazor client data to serve configuration
app.MapGet("/appsettings.server.json", async ctx => {
    var clientSettingsSection = app.Configuration.GetSection("ClientSettings");
    var clientJsonContent = Serialize(clientSettingsSection).ToString();
    await ctx.Response.WriteAsync(clientJsonContent);
});

...

static JToken Serialize(IConfiguration config) {
    JObject obj = new JObject();
    foreach (var child in config.GetChildren()) {
        obj.Add(child.Key, Serialize(child));
    }

    if (!obj.HasValues && config is IConfigurationSection section)
        return new JValue(section.Value);

    return obj;
}
```

