FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/FStore.Common/FStore.Common.csproj", "src/BuildingBlocks/FStore.Common/"]
COPY ["src/BuildingBlocks/FStore.EventBus/FStore.EventBus.csproj", "src/BuildingBlocks/FStore.EventBus/"]
COPY ["src/Services/Order/FStore.Order.Domain/FStore.Order.Domain.csproj", "src/Services/Order/FStore.Order.Domain/"]
COPY ["src/Services/Order/FStore.Order.Application/FStore.Order.Application.csproj", "src/Services/Order/FStore.Order.Application/"]
COPY ["src/Services/Order/FStore.Order.Infrastructure/FStore.Order.Infrastructure.csproj", "src/Services/Order/FStore.Order.Infrastructure/"]
COPY ["src/Services/Order/FStore.Order.Api/FStore.Order.Api.csproj", "src/Services/Order/FStore.Order.Api/"]

RUN dotnet restore "src/Services/Order/FStore.Order.Api/FStore.Order.Api.csproj"

COPY . .

WORKDIR "/src/src/Services/Order/FStore.Order.Api"
RUN dotnet publish "FStore.Order.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "FStore.Order.Api.dll"]