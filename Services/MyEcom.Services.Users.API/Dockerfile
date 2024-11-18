# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
RUN mkdir -p /home/app/.aspnet/https
COPY ./certs/.aspnet/https/Users.API.pfx /home/app/.aspnet/https/Users.API.pfx
RUN chown -R app:app /home/app/.aspnet/https
USER app
WORKDIR /app
EXPOSE 5010
EXPOSE 5011


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["Services/MyEcom.Services.Users.API/MyEcom.Services.Users.API.csproj", "Services/MyEcom.Services.Users.API/"]
RUN dotnet restore "./Services/MyEcom.Services.Users.API/MyEcom.Services.Users.API.csproj"
COPY . .
WORKDIR "/src/Services/MyEcom.Services.Users.API"
RUN dotnet build "./MyEcom.Services.Users.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyEcom.Services.Users.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyEcom.Services.Users.API.dll"]