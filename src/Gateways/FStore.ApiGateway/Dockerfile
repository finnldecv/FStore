FROM mcr.microsoft.com/dotnet/sdk:10.0.203 AS build
WORKDIR /src

COPY ["src/Gateways/FStore.ApiGateway/FStore.ApiGateway.csproj", "src/Gateways/FStore.ApiGateway/"]

RUN dotnet restore "src/Gateways/FStore.ApiGateway/FStore.ApiGateway.csproj"

COPY . .
WORKDIR "/src/src/Gateways/FStore.ApiGateway"
RUN dotnet publish "FStore.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0.203 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=HTTP://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "FStore.Basket.Api.dll"]