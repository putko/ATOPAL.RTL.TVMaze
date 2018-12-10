FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/Api/ApiGateway/ApiGateway.csproj", "src/Api/ApiGateway/"]
RUN dotnet restore "src/Api/ApiGateway/ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/Api/ApiGateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]