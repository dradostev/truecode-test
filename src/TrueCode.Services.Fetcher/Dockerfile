FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TrueCode.Services.Fetcher/TrueCode.Services.Fetcher.csproj", "src/TrueCode.Services.Fetcher/"]
RUN dotnet restore "src/TrueCode.Services.Fetcher/TrueCode.Services.Fetcher.csproj"
COPY . .
WORKDIR "/src/src/TrueCode.Services.Fetcher"
RUN dotnet build "TrueCode.Services.Fetcher.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TrueCode.Services.Fetcher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueCode.Services.Fetcher.dll"]