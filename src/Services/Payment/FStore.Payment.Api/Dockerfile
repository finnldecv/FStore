FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/FStore.Common/FStore.Common.csproj", "src/BuildingBlocks/FStore.Common/"]
COPY ["src/Services/Payment/FStore.Payment.Domain/FStore.Payment.Domain.csproj", "src/Services/Payment/FStore.Payment.Domain/"]
COPY ["src/Services/Payment/FStore.Payment.Application/FStore.Payment.Application.csproj", "src/Services/Payment/FStore.Payment.Application/"]
COPY ["src/Services/Payment/FStore.Payment.Infrastructure/FStore.Payment.Infrastructure.csproj", "src/Services/Payment/FStore.Payment.Infrastructure/"]
COPY ["src/Services/Payment/FStore.Payment.Api/FStore.Payment.Api.csproj", "src/Services/Payment/FStore.Payment.Api/"]

RUN dotnet restore "src/Services/Payment/FStore.Payment.Api/FStore.Payment.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/Payment/FStore.Payment.Api"
RUN dotnet publish "FStore.Payment.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "FStore.Payment.Api.dll"]