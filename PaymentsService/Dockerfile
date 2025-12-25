FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем SharedLibrary
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]

# Копируем проекты PaymentsService
COPY ["PaymentsService/src/PaymentsService.Api/PaymentsService.Api.csproj", "PaymentsService/src/PaymentsService.Api/"]
COPY ["PaymentsService/src/PaymentsService.Application/PaymentsService.Application.csproj", "PaymentsService/src/PaymentsService.Application/"]
COPY ["PaymentsService/src/PaymentsService.Domain/PaymentsService.Domain.csproj", "PaymentsService/src/PaymentsService.Domain/"]
COPY ["PaymentsService/src/PaymentsService.Infrastructure/PaymentsService.Infrastructure.csproj", "PaymentsService/src/PaymentsService.Infrastructure/"]

RUN dotnet restore "PaymentsService/src/PaymentsService.Api/PaymentsService.Api.csproj"

# Копируем все исходники
COPY SharedLibrary/ SharedLibrary/
COPY PaymentsService/ PaymentsService/

WORKDIR "/src/PaymentsService/src/PaymentsService.Api"
RUN dotnet build "PaymentsService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentsService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentsService.Api.dll"]
