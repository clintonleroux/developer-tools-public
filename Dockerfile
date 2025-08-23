# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# The base image runs as root by default; add a USER directive if a non-root user is required
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DeveloperTools/DeveloperTools.csproj", "src/DeveloperTools/"]
RUN dotnet restore "src/DeveloperTools/DeveloperTools.csproj"
COPY . .
#RUN dotnet build "src/DeveloperTools/DeveloperTools.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:UseAppHost=false

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/DeveloperTools/DeveloperTools.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Install WebAssembly workload
#RUN dotnet workload install wasm-tools

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "DeveloperTools.dll"]

# Use nginx for serving the Blazor WebAssembly app
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy the published files from the build stage
COPY --from=publish /app/publish/wwwroot .

# Expose ports
EXPOSE 80
EXPOSE 443

# Start nginx
CMD ["nginx", "-g", "daemon off;"]