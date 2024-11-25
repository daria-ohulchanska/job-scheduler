FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JobScheduler.Web/JobScheduler.Web.csproj", "JobScheduler.Web/"]
COPY ["JobScheduler/JobScheduler.Core.csproj", "JobScheduler/"]
COPY ["JobScheduler.Data/JobScheduler.Data.csproj", "JobScheduler.Data/"]
COPY ["JobScheduler.Shared/JobScheduler.Shared.csproj", "JobScheduler.Shared/"]
RUN dotnet restore "JobScheduler.Web/JobScheduler.Web.csproj"
COPY . .
WORKDIR "/src/JobScheduler.Web"
RUN dotnet build "JobScheduler.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "JobScheduler.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobScheduler.Web.dll"]
