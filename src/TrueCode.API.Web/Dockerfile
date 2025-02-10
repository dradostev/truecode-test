FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TrueCode.API.Web/TrueCode.API.Web.csproj", "TrueCode.API.Web/"]
RUN dotnet restore "TrueCode.API.Web/TrueCode.API.Web.csproj"
COPY . .
COPY ../protos /protos
WORKDIR "src/TrueCode.API.Web"
RUN dotnet build "TrueCode.API.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TrueCode.API.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueCode.API.Web.dll"]
