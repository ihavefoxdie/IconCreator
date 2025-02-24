ARG DOTNET_VERSION=8.0
ARG BUILD_CONFIGURATION=Release
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build

WORKDIR /src
COPY ["ToIcon.Api/", "ToIcon.Api/"]
COPY ["ToIcon.Library/", "ToIcon.Library/"]

WORKDIR /src/ToIcon.Api
RUN dotnet restore "./ToIcon.Api.csproj"
RUN dotnet build "./ToIcon.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/build
RUN dotnet publish "./ToIcon.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS run
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT [ "dotnet", "ToIcon.Api.dll" ]