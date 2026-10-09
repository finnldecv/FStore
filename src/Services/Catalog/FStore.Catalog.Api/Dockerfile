FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/Catalog/FStore.Catalog.Api/FStore.Catalog.Api.csproj", "src/Services/Catalog/FStore.Catalog.Api/"]
COPY ["src/Services/Catalog/FStore.Catalog.Application/FStore.Catalog.Application.csproj", "src/Services/Catalog/FStore.Catalog.Application/"]
COPY ["src/Services/Catalog/FStore.Catalog.Domain/FStore.Catalog.Domain.csproj", "src/Services/Catalog/FStore.Catalog.Domain/"]
COPY ["src/Services/Catalog/FStore.Catalog.Infrastructure/FStore.Catalog.Infrastructure.csproj", "src/Services/Catalog/FStore.Catalog.Infrastructure/"]
COPY ["src/BuildingBlocks/FStore.Common/FStore.Common.csproj", "src/BuildingBlocks/FStore.Common/"]
COPY ["src/BuildingBlocks/FStore.EventBus/FStore.EventBus.csproj", "src/BuildingBlocks/FStore.EventBus/"]

RUN dotnet restore "src/Services/Catalog/FStore.Catalog.Api/FStore.Catalog.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Catalog/FStore.Catalog.Api"
RUN dotnet publish "FStore.Catalog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8000

ENV ASPNETCORE_URLS=HTTP://+:8000

ENTRYPOINT ["dotnet", "FStore.Catalog.Api.dll"]