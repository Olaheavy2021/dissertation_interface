#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Dissertation_API/Dissertation_API.csproj", "Dissertation_API/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "Dissertation_API/Dissertation_API.csproj"
COPY . .
WORKDIR "/src/Dissertation_API"
RUN dotnet build "Dissertation_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dissertation_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://*:8000;http://*:8001
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
EXPOSE 8000
EXPOSE 8001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dissertation_API.dll"]