# Multi-stage build pro .NET aplikaci

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopíruj project file
COPY ["MiniMES.Api.csproj", "./"]

# Obnov NuGet balíčky
RUN dotnet restore "MiniMES.Api.csproj"

# Kopíruj zbytek kódu
COPY . .

# Builduj aplikaci
RUN dotnet build "MiniMES.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "MiniMES.Api.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Kopíruj published aplikaci
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=10s --timeout=3s --start-period=5s --retries=3 \
  CMD dotnet --version || exit 1

# Port
EXPOSE 8080

# Nastavení prostředí
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Start aplikace
ENTRYPOINT ["dotnet", "MiniMES.Api.dll"]
