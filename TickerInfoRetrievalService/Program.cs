using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;
using TickerInfoRetrievalService.Services;
using TickerInfoRetrievalService.Services.Interfaces;

namespace TickerInfoRetrievalService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            var tickerInfoEndpoint = Environment.GetEnvironmentVariable("TICKERINFOENDPOINT");
            var apiKKey = Environment.GetEnvironmentVariable("APIKEY");
            var dbEndpoint = Environment.GetEnvironmentVariable("DBENDPOINT");
            var service = new ServiceCollection()
                .AddScoped<IInfoRetrievalService>(_ => new InfoRetrievalService(tickerInfoEndpoint, apiKKey))
                .AddScoped<IDBCommunicationService>(_ => new DBCommunicationService(dbEndpoint))
                .AddScoped<InfoScraperService>()
                .BuildServiceProvider();

            var scraper = service.GetService<InfoScraperService>();
            var tickers = await service.GetService<IDBCommunicationService>().GetTickers();
            await scraper.CreateBrowser();
            foreach (var ticker in tickers)
            {
                var info = await scraper.ScrapeByTicker(ticker.Ticker);
                Log.Information($"{ticker.Ticker} {info.Volume.ToString()}");
            }
            Task.Delay(-1).Wait();

        }
    }
}
