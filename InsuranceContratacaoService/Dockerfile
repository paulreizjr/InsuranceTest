FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["InsuranceContratacaoService/InsuranceContratacaoService.csproj", "InsuranceContratacaoService/"]
COPY ["InsuranceCoreBusiness/InsuranceCoreBusiness.csproj", "InsuranceCoreBusiness/"]
RUN dotnet restore "./InsuranceContratacaoService/InsuranceContratacaoService.csproj"
COPY . .
WORKDIR "/src/InsuranceContratacaoService"
RUN dotnet build "./InsuranceContratacaoService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InsuranceContratacaoService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InsuranceContratacaoService.dll"]
