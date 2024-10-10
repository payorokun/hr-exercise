#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Crud-Api/Crud.Webapi/Crud.Webapi.csproj", "Crud-Api/Crud.Webapi/"]
COPY ["Crud-Api/Crud.Application/Crud.Application.csproj", "Crud-Api/Crud.Application/"]
COPY ["Crud-Api/Crud.Domain/Crud.Domain.csproj", "Crud-Api/Crud.Domain/"]
COPY ["Crud-Api/Crud.Infrastructure/Crud.Infrastructure.csproj", "Crud-Api/Crud.Infrastructure/"]
RUN dotnet restore "./Crud-Api/Crud.Webapi/Crud.Webapi.csproj"
COPY . .
WORKDIR "/src/Crud-Api/Crud.Webapi"
RUN dotnet build "./Crud.Webapi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Crud.Webapi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crud.Webapi.dll"]