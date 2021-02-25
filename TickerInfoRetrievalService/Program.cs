using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using Serilog;
using System;
using System.Diagnostics;
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
       
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var tickerInfoEndpoint = Environment.GetEnvironmentVariable("TICKERINFOENDPOINT");
                var apiKKey = Environment.GetEnvironmentVariable("APIKEY");
                var dbEndpoint = Environment.GetEnvironmentVariable("DBENDPOINT");
                var scraperService = new InfoScraperService();
                var infoRetrievalService = new InfoRetrievalService(tickerInfoEndpoint, apiKKey);
                var dbCommunicationService = new DBCommunicationService(dbEndpoint);
                services.AddSingleton<IInfoScraperService>(_ => scraperService);
                services.AddScoped<IInfoRetrievalService>(_ => infoRetrievalService);
                services.AddScoped<IDBCommunicationService>(_ => dbCommunicationService);
                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionScopedJobFactory();
                    
                    var jobKey = new JobKey("InfoRetrievalJob");
                    q.AddJob<InfoRetrievalJob>(opts => 
                    {
                        opts.WithIdentity(jobKey);
                    });
                    q.AddTrigger(opts => opts.ForJob(jobKey).WithDailyTimeIntervalSchedule(s =>
                                    s.WithIntervalInHours(24)
                                    .OnMondayThroughFriday()
                                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(21, 30))
                                    .InTimeZone(TimeZoneInfo.Utc)
                                )
                    .WithIdentity("InfoRetrievalJobTrigger")
                    );

                });
                services.AddQuartzHostedService(
                    q => q.WaitForJobsToComplete = true);
            });
        /*
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
            var tickers = await dbService.GetTickers();
            //var remaining = tickers.SkipWhile(i => i.Ticker != "SEE");
            foreach (var ticker in tickers)
            {
                var info = await scraper.ScrapeByTicker(ticker.Ticker);
                if (info != null)
                {
                    //await dbService.InsertDailyInfoByTicker(info);
                }
                
            }
            //SetTimer(new TimeSpan(21, 30, 0), dbService, scraper);
            Task.Delay(-1).Wait();

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
                if (info != null)
                {
                    await dbService.InsertDailyInfoByTicker(info);
                }
            }
            Log.Information($"{DateTime.UtcNow.DayOfWeek} Info Complete");
        }

        */
        

        public class QuartzJobRunner : IJob
        {
            private readonly IServiceProvider _serviceProvider;
            public QuartzJobRunner(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public async Task Execute(IJobExecutionContext context)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var jobType = context.JobDetail.JobType;
                    var job = scope.ServiceProvider.GetRequiredService(jobType) as IJob;

                    await job.Execute(context);
                }
            }
        }

        public class InfoJobFactory : IJobFactory
        {
            private readonly IServiceProvider serviceProvider;
            public InfoJobFactory(IServiceProvider serviceProvider)
            {
                this.serviceProvider = serviceProvider;
            }

            public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
            {
                return this.serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
            }

            public void ReturnJob(IJob job) { }

        }

        [DisallowConcurrentExecution]
        public class InfoRetrievalJob : IJob
        {
            private readonly IDBCommunicationService dBCommunicationService;
            private readonly IInfoScraperService scraperService;
            public InfoRetrievalJob(IDBCommunicationService dBCommunicationService, IInfoScraperService scraperService)
            {
                this.dBCommunicationService = dBCommunicationService;
                this.scraperService = scraperService;
            }

            public async Task Execute(IJobExecutionContext context)
            {
                var tickers = await dBCommunicationService.GetTickers();
                foreach (var ticker in tickers)
                {
                    var info = await scraperService.ScrapeByTicker(ticker.Ticker);
                    await dBCommunicationService.InsertDailyInfoByTicker(info);
                }
                //return Task.CompletedTask;
            }
        }

    }
}
