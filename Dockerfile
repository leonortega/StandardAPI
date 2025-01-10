# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution files
COPY *.sln ./
COPY Domain/*.csproj ./Domain/
COPY Application/*.csproj ./Application/
COPY Infraestructure/*.csproj ./Infraestructure/
COPY Shared/*.csproj ./Shared/
COPY Test/*.csproj ./Test/
COPY StandardAPI/*.csproj ./StandardAPI/

# Restore dependencies
RUN dotnet restore

# Copy the full source code
COPY . .

# Build the application
RUN dotnet build --no-restore -c Release

# Publish the application
RUN dotnet publish --no-restore -c Release -o /out

# Stage 2: Runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /out .

# Set environment variables for logging and configuration
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    DOTNET_GENERATE_ASPNET_CERTIFICATE=false

# Expose the default HTTP and HTTPS ports
EXPOSE 80
EXPOSE 443

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "StandardAPI.API.dll"]
