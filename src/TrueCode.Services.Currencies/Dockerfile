FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TrueCode.Services.Currencies/TrueCode.Services.Currencies.csproj", "TrueCode.Services.Currencies/"]
RUN dotnet restore "TrueCode.Services.Currencies/TrueCode.Services.Currencies.csproj"
COPY . .
WORKDIR "/src/TrueCode.Services.Currencies"
RUN dotnet build "TrueCode.Services.Currencies.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TrueCode.Services.Currencies.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueCode.Services.Currencies.dll"]
