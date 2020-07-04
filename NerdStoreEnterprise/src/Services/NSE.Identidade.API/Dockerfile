FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env
WORKDIR /app

COPY ./Services/NSE.Identidade.API/.dockerignore ./
COPY ["./BuildingBlocks/NSE.Core/NSE.Core.csproj", "/BuildingBlocks/NSE.Core/NSE.Core.csproj"]
COPY ["./BuildingBlocks/NSE.WebAPI.Core/NSE.WebAPI.Core.csproj", "/BuildingBlocks/NSE.WebAPI.Core/NSE.WebAPI.Core.csproj"]


# COPY ./BuildingBlocks/NSE.Core/NSE.Core.csproj ./
COPY Services/NSE.Identidade.API/NSE.Identidade.API.csproj ./

RUN ls -l


# RUN dotnet restore

# # Build da aplicacao
# COPY . ./
# RUN dotnet publish -c Release -o out

# # Build da imagem
# FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
# WORKDIR /app
# COPY --from=build-env /app/out .
# ENTRYPOINT ["dotnet", "NSE.Identidade.API.dll"]






# FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 443

# FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
# WORKDIR /src
# COPY ["src/Api.Sample.WebApiConsumer/Api.Sample.WebApiConsumer.csproj", "src/Api.Sample.WebApiConsumer/"]
# COPY ["src/Api.Sample.Domain/Api.Sample.Domain.csproj", "src/Api.Sample.Domain/"]
# COPY ["src/Api.Sample.Infrastructure/Api.Sample.Infrastructure.csproj", "src/Api.Sample.Infrastructure/"]
# COPY ["src/Api.Sample.Application/Api.Sample.Application.csproj", "src/Api.Sample.Application/"]
# RUN dotnet restore "src/Api.Sample.WebApiConsumer/Api.Sample.WebApiConsumer.csproj"
# COPY . .
# WORKDIR "/src/src/Api.Sample.WebApiConsumer"
# RUN dotnet build "Api.Sample.WebApiConsumer.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "Api.Sample.WebApiConsumer.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Api.Sample.WebApiConsumer.dll"]
