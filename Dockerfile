FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY scr/JobScheduler.sln .
COPY scr/JobScheduler.Web/JobScheduler.Web.csproj JobScheduler.Web/
RUN dotnet restore JobScheduler.sln

COPY scr/ .
WORKDIR /src/JobScheduler.Web
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "JobScheduler.Web.dll"]