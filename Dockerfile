#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /drone/src
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /drone/src
COPY ["droneService.csproj", "."]
RUN dotnet restore "./droneService.csproj"
COPY . .
WORKDIR "/drone/src/."
RUN dotnet build "droneService.csproj"

FROM build AS publish
RUN dotnet publish "droneService.csproj"

FROM base AS final
WORKDIR /drone/src
ENTRYPOINT ["nohup ","dotnet", "./bin/Debug/net7.0/droneService.dll"]