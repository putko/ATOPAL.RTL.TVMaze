FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 65022
EXPOSE 44338

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Services/TVMaze.Scrapper/TVMaze.Scrapper.csproj src/Services/TVMaze.Scrapper/
COPY src/BuildingBlocks/EventBus/EventBusRabbitMQ/RabbitMQ.csproj src/BuildingBlocks/EventBus/EventBusRabbitMQ/
COPY src/BuildingBlocks/EventBus/EventBus.Common/Common.csproj src/BuildingBlocks/EventBus/EventBus.Common/
RUN dotnet restore src/Services/TVMaze.Scrapper/TVMaze.Scrapper.csproj
COPY . .
WORKDIR /src/src/Services/TVMaze.Scrapper
RUN dotnet build TVMaze.Scrapper.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TVMaze.Scrapper.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.dll"]
