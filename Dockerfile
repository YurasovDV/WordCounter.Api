# Dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /src
COPY ["WordCounter.API.csproj", "."]
COPY ["../WordCounter.Common/WordCounter.Common.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app .

ENV RabbitMqHost rabbit_mq
ENV RabbitMqPort 5672
ENV RabbitMqUser admin
ENV RabbitMqPass demo
ENV ASPNETCORE_URLS http://*:5000


ENTRYPOINT ["dotnet", "WordCounter.API.dll"]