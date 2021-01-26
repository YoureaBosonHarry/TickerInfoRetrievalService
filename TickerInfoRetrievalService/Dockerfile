#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim  AS base
WORKDIR /app

ARG CHROME_VERSION="81.0.4044.138-1"
RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget --no-verbose -O /tmp/chrome.deb http://dl.google.com/linux/chrome/deb/pool/main/g/google-chrome-stable/google-chrome-stable_${CHROME_VERSION}_amd64.deb \
&& apt-get update \
&& apt-get install -y /tmp/chrome.deb --no-install-recommends --allow-downgrades fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf \
&& rm /tmp/chrome.deb

ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome"

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim  AS build
WORKDIR /src
COPY ["TickerInfoRetrievalService.csproj", "TickerInfoRetrievalService/"]
RUN dotnet restore "TickerInfoRetrievalService/TickerInfoRetrievalService.csproj"
COPY . .
WORKDIR "/src/TickerInfoRetrievalService"
COPY . .
RUN dotnet build "TickerInfoRetrievalService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TickerInfoRetrievalService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TickerInfoRetrievalService.dll"]