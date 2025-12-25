FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем SharedLibrary
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]

# Копируем проекты OrdersService
COPY ["OrdersService/src/OrdersService.Api/OrdersService.Api.csproj", "OrdersService/src/OrdersService.Api/"]
COPY ["OrdersService/src/OrdersService.Application/OrdersService.Application.csproj", "OrdersService/src/OrdersService.Application/"]
COPY ["OrdersService/src/OrdersService.Domain/OrdersService.Domain.csproj", "OrdersService/src/OrdersService.Domain/"]
COPY ["OrdersService/src/OrdersService.Infrastructure/OrdersService.Infrastructure.csproj", "OrdersService/src/OrdersService.Infrastructure/"]

RUN dotnet restore "OrdersService/src/OrdersService.Api/OrdersService.Api.csproj"

# Копируем все исходники
COPY SharedLibrary/ SharedLibrary/
COPY OrdersService/ OrdersService/

WORKDIR "/src/OrdersService/src/OrdersService.Api"
RUN dotnet build "OrdersService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "OrdersService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrdersService.Api.dll"]
