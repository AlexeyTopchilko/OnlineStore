#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CartMicroservice.API/CartMicroservice.API.csproj", "CartMicroservice.API/"]
COPY ["CartMicroservice.Service/CartMicroservice.Service.csproj", "CartMicroservice.Service/"]
COPY ["CartMicroservice.Repository/CartMicroservice.Repository.csproj", "CartMicroservice.Repository/"]
COPY ["CartMicroservice.Domain/CartMicroservice.Domain.csproj", "CartMicroservice.Domain/"]
RUN dotnet restore "CartMicroservice.API/CartMicroservice.API.csproj"
COPY . .
WORKDIR "/src/CartMicroservice.API"
RUN dotnet build "CartMicroservice.API.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "CartMicroservice.API.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartMicroservice.API.dll"]