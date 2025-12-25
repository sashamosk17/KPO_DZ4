FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем проект ApiGateway
COPY ["ApiGateway/src/ApiGateway.Api/ApiGateway.Api.csproj", "ApiGateway/src/ApiGateway.Api/"]

RUN dotnet restore "ApiGateway/src/ApiGateway.Api/ApiGateway.Api.csproj"

# Копируем все исходники
COPY ApiGateway/ ApiGateway/

WORKDIR "/src/ApiGateway/src/ApiGateway.Api"
RUN dotnet build "ApiGateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.Api.dll"]
