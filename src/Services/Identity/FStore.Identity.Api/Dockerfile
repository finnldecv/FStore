FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/FStore.Common/FStore.Common.csproj", "src/BuildingBlocks/FStore.Common/"]
COPY ["src/Services/Identity/FStore.Identity.Domain/FStore.Identity.Domain.csproj", "src/Services/Identity/FStore.Identity.Domain/"]
COPY ["src/Services/Identity/FStore.Identity.Application/FStore.Identity.Application.csproj", "src/Services/Identity/FStore.Identity.Application/"]
COPY ["src/Services/Identity/FStore.Identity.Infrastructure/FStore.Identity.Infrastructure.csproj", "src/Services/Identity/FStore.Identity.Infrastructure/"]
COPY ["src/Services/Identity/FStore.Identity.Api/FStore.Identity.Api.csproj", "src/Services/Identity/FStore.Identity.Api/"]

RUN dotnet restore "src/Services/Identity/FStore.Identity.Api/FStore.Identity.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Identity/FStore.Identity.Api"
RUN dotnet publish "FStore.Identity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "FStore.Identity.Api.dll"]