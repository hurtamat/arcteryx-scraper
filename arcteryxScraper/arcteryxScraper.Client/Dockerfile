FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["arcteryxScraper/arcteryxScraper.Client/arcteryxScraper.Client.csproj", "arcteryxScraper/arcteryxScraper.Client/"]
RUN dotnet restore "arcteryxScraper/arcteryxScraper.Client/arcteryxScraper.Client.csproj"
COPY . .
WORKDIR "/src/arcteryxScraper/arcteryxScraper.Client"
RUN dotnet build "./arcteryxScraper.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./arcteryxScraper.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "arcteryxScraper.Client.dll"]
