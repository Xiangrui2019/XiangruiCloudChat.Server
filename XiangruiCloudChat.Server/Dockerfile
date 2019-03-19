FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY ["XiangruiCloudChat.Server/XiangruiCloudChat.Server.csproj", "XiangruiCloudChat.Server/"]
RUN dotnet restore "XiangruiCloudChat.Server/XiangruiCloudChat.Server.csproj"
COPY . .
WORKDIR "/src/XiangruiCloudChat.Server"
RUN dotnet build "XiangruiCloudChat.Server.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "XiangruiCloudChat.Server.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "XiangruiCloudChat.Server.dll"]