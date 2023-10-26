#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Dissertation.Api/Dissertation.Api.csproj", "Dissertation.Api/"]
COPY ["Dissertation.Application/Dissertation.Application.csproj", "Dissertation.Application/"]
COPY ["Dissertation.Domain/Dissertation.Domain.csproj", "Dissertation.Domain/"]
COPY ["Dissertation.Infrastructure/Dissertation.Infrastructure.csproj", "Dissertation.Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "Dissertation.Api/Dissertation.Api.csproj"
COPY . .
WORKDIR "/src/Dissertation.Api"
RUN dotnet build "Dissertation.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dissertation.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://*:8000;http://*:8001
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
EXPOSE 8000
EXPOSE 8001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dissertation.Api.dll"]