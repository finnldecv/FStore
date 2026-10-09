FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/Basket/FStore.Basket.Api/FStore.Basket.Api.csproj", "src/Services/Basket/FStore.Basket.Api/"]
COPY ["src/Services/Basket/FStore.Basket.Application/FStore.Basket.Application.csproj", "src/Services/Basket/FStore.Basket.Application/"]
COPY ["src/Services/Basket/FStore.Basket.Domain/FStore.Basket.Domain.csproj", "src/Services/Basket/FStore.Basket.Domain/"]
COPY ["src/Services/Basket/FStore.Basket.Infrastructure/FStore.Basket.Infrastructure.csproj", "src/Services/Basket/FStore.Basket.Infrastructure/"]
COPY ["src/BuildingBlocks/FStore.Common/FStore.Common.csproj", "src/BuildingBlocks/FStore.Common/"]
COPY ["src/BuildingBlocks/FStore.EventBus/FStore.EventBus.csproj", "src/BuildingBlocks/FStore.EventBus/"]

RUN dotnet restore "src/Services/Basket/FStore.Basket.Api/FStore.Basket.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Basket/FStore.Basket.Api"
RUN dotnet publish "FStore.Basket.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=HTTP://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "FStore.Basket.Api.dll"]