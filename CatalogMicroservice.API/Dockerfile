#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CatalogMicroservice.API/CatalogMicroservice.API.csproj", "CatalogMicroservice.API/"]
COPY ["CatalogMicroservice.Service/CatalogMicroservice.Service.csproj", "CatalogMicroservice.Service/"]
COPY ["CatalogMicroservice.Repository/CatalogMicroservice.Repository.csproj", "CatalogMicroservice.Repository/"]
COPY ["CatalogMicroservice.Domain/CatalogMicroservice.Domain.csproj", "CatalogMicroservice.Domain/"]
RUN dotnet restore "CatalogMicroservice.API/CatalogMicroservice.API.csproj"
COPY . .
WORKDIR "/src/CatalogMicroservice.API"
RUN dotnet build "CatalogMicroservice.API.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogMicroservice.API.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogMicroservice.API.dll"]