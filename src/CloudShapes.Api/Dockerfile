FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
RUN apt-get update
RUN apt-get install -y jq
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY readme.md /src/readme.md
COPY ["src/CloudShapes.Api/CloudShapes.Api.csproj", "src/CloudShapes.Api/"]
COPY ["src/CloudShapes.Application/CloudShapes.Application.csproj", "src/CloudShapes.Application/"]
COPY ["src/CloudShapes.Data/CloudShapes.Data.csproj", "src/CloudShapes.Data/"]
RUN dotnet restore "./src/CloudShapes.Api/CloudShapes.Api.csproj"
COPY . .
WORKDIR "/src/src/CloudShapes.Api"
RUN dotnet build "./CloudShapes.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CloudShapes.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudShapes.Api.dll"]