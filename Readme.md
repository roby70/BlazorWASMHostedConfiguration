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

## Override appsettings.json on server side

Since the host server that has the responsability to serve single page application to the browser, it could be used to intercept the request of the appsettings.json file and return a processed file based on his own configuration.

