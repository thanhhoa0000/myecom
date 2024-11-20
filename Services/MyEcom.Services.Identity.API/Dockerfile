FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
RUN mkdir -p /home/app/.aspnet/https
COPY ./certs/.aspnet/https/Identity.API.pfx /home/app/.aspnet/https/Identity.API.pfx
RUN chown -R app:app /home/app/.aspnet/https
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY [".env", "."]
COPY ["Services/MyEcom.Services.Identity.API/MyEcom.Services.Identity.API.csproj", "Services/MyEcom.Services.Identity.API/"]
RUN dotnet restore "Services/MyEcom.Services.Identity.API/MyEcom.Services.Identity.API.csproj"
COPY . .
WORKDIR "/src/Services/MyEcom.Services.Identity.API"
RUN dotnet build "MyEcom.Services.Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MyEcom.Services.Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY [".env", "/app/.env"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyEcom.Services.Identity.API.dll"]
