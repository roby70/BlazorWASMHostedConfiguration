#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlazorApp/Server/BlazorApp.Server.csproj", "BlazorApp/Server/"]
COPY ["BlazorApp/Client/BlazorApp.Client.csproj", "BlazorApp/Client/"]
COPY ["BlazorApp/Shared/BlazorApp.Shared.csproj", "BlazorApp/Shared/"]
RUN dotnet restore "BlazorApp/Server/BlazorApp.Server.csproj"
COPY . .
WORKDIR "/src/BlazorApp/Server"
RUN dotnet build "BlazorApp.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorApp.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorApp.Server.dll"]