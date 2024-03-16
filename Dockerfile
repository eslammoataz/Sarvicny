#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Sarvicny.Api/Sarvicny.Api.csproj", "Sarvicny.Api/"]
COPY ["Sarvicny.Application/Sarvicny.Application.csproj", "Sarvicny.Application/"]
COPY ["Sarvicny.Contracts/Sarvicny.Contracts.csproj", "Sarvicny.Contracts/"]
COPY ["Sarvicny.Domain/Sarvicny.Domain.csproj", "Sarvicny.Domain/"]
#COPY ["Sarvicny.Infrastructure/Sarvicny.Infrastructure.csproj", "Sarvicny.Infrastructure/"]
RUN dotnet restore "Sarvicny.Api/Sarvicny.Api.csproj"
COPY . .
WORKDIR "/src/Sarvicny.Api"
RUN dotnet build "Sarvicny.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sarvicny.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sarvicny.Api.dll"]


