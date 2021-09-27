#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OrderMicroservice.API/OrderMicroservice.API.csproj", "OrderMicroservice.API/"]
COPY ["OrderMicroservice.Service/OrderMicroservice.Service.csproj", "OrderMicroservice.Service/"]
COPY ["OrderMicroservice.Repository/OrderMicroservice.Repository.csproj", "OrderMicroservice.Repository/"]
COPY ["OrderMicroservice.Domain/OrderMicroservice.Domain.csproj", "OrderMicroservice.Domain/"]
RUN dotnet restore "OrderMicroservice.API/OrderMicroservice.API.csproj"
COPY . .
WORKDIR "/src/OrderMicroservice.API"
RUN dotnet build "OrderMicroservice.API.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "OrderMicroservice.API.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderMicroservice.API.dll"]