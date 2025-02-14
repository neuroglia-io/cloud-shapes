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
RUN echo "Placeholder README" > /src/readme.md
COPY ["src/CloudShapes.Api/CloudShapes.Api.csproj", "src/CloudShapes.Api/"]
COPY ["src/CloudShapes.Application/CloudShapes.Application.csproj", "src/CloudShapes.Application/"]
COPY ["src/CloudShapes.Integration/CloudShapes.Integration.csproj", "src/CloudShapes.Integration/"]
COPY ["src/CloudShapes.Data/CloudShapes.Data.csproj", "src/CloudShapes.Data/"]
COPY ["src/CloudShapes.Dashboard/CloudShapes.Dashboard.csproj", "src/CloudShapes.Dashboard/"]
COPY ["src/CloudShapes.Api.Client/CloudShapes.Api.Client.csproj", "src/CloudShapes.Api.Client/"]
COPY ["src/CloudShapes.Dashboard.StateManagement/CloudShapes.Dashboard.StateManagement.csproj", "src/CloudShapes.Dashboard.StateManagement/"]
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