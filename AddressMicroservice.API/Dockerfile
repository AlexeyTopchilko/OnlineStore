#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AddressMicroservice.API/AddressMicroservice.API.csproj", "AddressMicroservice.API/"]
COPY ["AddressMicroservice.Service/AddressMicroservice.Service.csproj", "AddressMicroservice.Service/"]
COPY ["AddressMicroservice.Repository/AddressMicroservice.Repository.csproj", "AddressMicroservice.Repository/"]
COPY ["AddressMicroservice.Domain/AddressMicroservice.Domain.csproj", "AddressMicroservice.Domain/"]
RUN dotnet restore "AddressMicroservice.API/AddressMicroservice.API.csproj"
COPY . .
WORKDIR "/src/AddressMicroservice.API"
RUN dotnet build "AddressMicroservice.API.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "AddressMicroservice.API.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AddressMicroservice.API.dll"]