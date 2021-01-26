using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TickerInfoRetrievalService.Models;
using TickerInfoRetrievalService.Services;
using TickerInfoRetrievalService.Services.Interfaces;

namespace TickerInfoRetrievalService
{
    class Program
    {
        private static System.Threading.Timer timer;
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
            Log.Information("Starting Info Retrieval Service");
            var scraper = service.GetService<InfoScraperService>();
            var dbService = service.GetService<IDBCommunicationService>();
            await scraper.CreateBrowser();
            SetTimer(new TimeSpan(21, 30, 0), dbService, scraper);
            Task.Delay(-1).Wait();

        }
        public static void SetTimer(TimeSpan alertTime, IDBCommunicationService dbService, IInfoScraperService scraperService)
        {
            TimeSpan timeRemaining = alertTime - DateTime.UtcNow.TimeOfDay;
            if (timeRemaining < TimeSpan.Zero)
            {
                return;
            }
            timer = new System.Threading.Timer(async x =>
            {
                await ManageAlerts(dbService, scraperService);
            }, null, timeRemaining, Timeout.InfiniteTimeSpan);
        }

        private static async Task ManageAlerts(IDBCommunicationService dbService, IInfoScraperService scraperService)
        {
            if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday || DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
            {
                Log.Information($"{DateTime.UtcNow.DayOfWeek}: Market Closed");
                return;
            }
            Log.Information($"{DateTime.UtcNow.DayOfWeek}: Sending DailyInfo");
            var tickers = await dbService.GetTickers();
            foreach (var ticker in tickers)
            {
                var info = await scraperService.ScrapeByTicker(ticker.Ticker);
                await dbService.InsertDailyInfoByTicker(info);
            }
            Log.Information($"{DateTime.UtcNow.DayOfWeek} Info Complete");
        }

    }
}
