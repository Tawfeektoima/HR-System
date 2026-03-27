# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all projects
COPY ["HRSystem.Core/HRSystem.Core.csproj", "HRSystem.Core/"]
COPY ["HRSystem.Infrastructure/HRSystem.Infrastructure.csproj", "HRSystem.Infrastructure/"]
COPY ["HRSystem.API/HRSystem.API.csproj", "HRSystem.API/"]

# Restore dependencies
RUN dotnet restore "HRSystem.API/HRSystem.API.csproj"

# Copy the rest of the code
COPY . .

# Build and Publish the API project
WORKDIR "/src/HRSystem.API"
RUN dotnet publish "HRSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080

ENTRYPOINT ["dotnet", "HRSystem.API.dll"]
