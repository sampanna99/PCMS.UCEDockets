FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 5023

ENV ASPNETCORE_URLS=http://+:5023
ENV UCEDockets__LocalSyncPath=/data/sync
ENV UCEDockets__Sqlite__Path=/data/db/ucedockets.sqlite

RUN mkdir -p /data/sync
RUN mkdir -p /data/db

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && \
    chown -R appuser /app && \
    chown -R appuser /data/sync && \
    chown -R appuser /data/db

USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src

# we need dotnet-xscgen to build, install from tools manifest
COPY [".config/dotnet-tools.json", ".config/dotnet-tools.json"]
RUN dotnet tool restore

COPY ["./src/PCMS.UCEDockets/PCMS.UCEDockets.csproj", "./src/PCMS.UCEDockets/"]
RUN dotnet restore "./src/PCMS.UCEDockets/PCMS.UCEDockets.csproj"
COPY ["./src/PCMS.UCEDockets", "./src/PCMS.UCEDockets"]
WORKDIR "/src/src/PCMS.UCEDockets/"
RUN dotnet build "PCMS.UCEDockets.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PCMS.UCEDockets.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/config

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PCMS.UCEDockets.dll"]

