# Use the official ASP.NET Core 6.0 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the application code and build the project
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core 6.0 runtime image as the final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "encrypt-server.dll"]
