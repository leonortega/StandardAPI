# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install curl
USER root
RUN apt-get update && apt-get install -y curl

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["StandardAPI/StandardAPI.API.csproj", "StandardAPI/"]
COPY ["Application/StandardAPI.Application.csproj", "Application/"]
COPY ["Domain/StandardAPI.Domain.csproj", "Domain/"]
COPY ["Infraestructure/StandardAPI.Infraestructure.csproj", "Infraestructure/"]
RUN dotnet restore "./StandardAPI/StandardAPI.API.csproj"
COPY . .
WORKDIR "/src/StandardAPI"
RUN dotnet build "./StandardAPI.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StandardAPI.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StandardAPI.API.dll"]