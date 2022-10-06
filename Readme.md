# Manage Blazor WASM configuration

In this repository you'll find a sample about managing the configuration of a Blazor WASM application providing settings from the server side.

## Why getting configuration from the host server

The use case was an application organized by microservices based on Docker containers. In the specific case in the Blazor WASM app there are some settings like the uri of the OpenId Connect identity provider that can change between various environments. 

With docker, every ASP.NET app that run in a container could be configured, in addition to the appsetings.json file present in the project, by environment variables defined in the image. 

I thinked that would be nice if I can use the same approach to configure the Blazor app in the same way.

## Override appsettings.json on server side

Since the host server that has the responsability to serve single page application to the browser, it could be used to intercept the request of the appsettings.json file and return a processed file based on his own configuration.

