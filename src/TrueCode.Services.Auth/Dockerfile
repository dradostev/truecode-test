FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TrueCode.Services.Auth/TrueCode.Services.Auth.csproj", "TrueCode.Services.Auth/"]
RUN dotnet restore "TrueCode.Services.Auth/TrueCode.Services.Auth.csproj"
COPY . .
COPY ../../protos ../
WORKDIR "/src/TrueCode.Services.Auth"
RUN dotnet build "TrueCode.Services.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TrueCode.Services.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueCode.Services.Auth.dll"]
