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
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            var tickerInfoEndpoint = Environment.GetEnvironmentVariable("TICKERINFOENDPOINT");
            var apiKKey = Environment.GetEnvironmentVariable("APIKEY");
            var service = new ServiceCollection()
                .AddScoped<IInfoRetrievalService>(_ => new InfoRetrievalService(tickerInfoEndpoint, apiKKey))
                .BuildServiceProvider();

            var infoService = service.GetService<IInfoRetrievalService>();
            infoService.GetDailyInfoByTicker("aapl");
            Task.Delay(-1).Wait();

        }
    }
}
