﻿FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TrueCode.Services.Migrator/TrueCode.Services.Migrator.csproj", "src/TrueCode.Services.Migrator/"]
RUN dotnet restore "src/TrueCode.Services.Migrator/TrueCode.Services.Migrator.csproj"
COPY . .
WORKDIR "/src/src/TrueCode.Services.Migrator"
RUN dotnet build "TrueCode.Services.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TrueCode.Services.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrueCode.Services.Migrator.dll"]
