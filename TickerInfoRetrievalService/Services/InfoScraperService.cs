using HtmlAgilityPack;
using PuppeteerSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TickerInfoRetrievalService.Models;
using TickerInfoRetrievalService.Services.Interfaces;

namespace TickerInfoRetrievalService.Services
{
    class InfoScraperService : IInfoScraperService
    {
        private readonly ILogger logger;
        public InfoScraperService()
        {
            this.logger = Log.ForContext<InfoScraperService>();
        }

        public async Task CreateBrowser()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
        }

        public async Task<YahooSummaryModel> ScrapeByTicker(string ticker)
        {
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true, Args = new string[] { "--no-sandbox" } }))
            using (var page = await browser.NewPageAsync())
            {
                page.DefaultTimeout = 30000;
                this.logger.Debug($"Attempting To Scrape Info For {ticker}");
                await page.GoToAsync($"https://finance.yahoo.com/quote/{ticker}/history?p={ticker}");
                this.logger.Debug($"Successfully Scraped Info For {ticker}");
                string content = await page.GetContentAsync();
                return ParseHtml(ticker, content);
            }
        }

        private YahooSummaryModel ParseHtml(string ticker, string html)
        {
            DateTime dateAdded = new DateTime();
            decimal high = decimal.Zero;
            decimal low = decimal.Zero;
            decimal open = decimal.Zero;
            decimal close = decimal.Zero;
            decimal adjClose = decimal.Zero;
            int volume = 0;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//table")
                .Descendants("tr")
                .Skip(1)
                .Where(tr => tr.Elements("td").Count() > 1)
                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                .ToList();
            if (htmlBody.Count() > 0 && htmlBody.FirstOrDefault().Count() > 6)
            {
                var summaryModel = new YahooSummaryModel();
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                DateTime.TryParse(htmlBody.FirstOrDefault().ElementAtOrDefault(0).ToString(), out dateAdded);
                decimal.TryParse(htmlBody.FirstOrDefault().ElementAtOrDefault(1).ToString(), out open);
                decimal.TryParse(htmlBody.FirstOrDefault().ElementAtOrDefault(2).ToString(), out high);
                decimal.TryParse(htmlBody.FirstOrDefault().ElementAtOrDefault(3).ToString(), out low);
                decimal.TryParse(htmlBody.FirstOrDefault().ElementAtOrDefault(4).ToString(), out close);
                decimal.TryParse(htmlBody.FirstOrDefault().ElementAtOrDefault(5).ToString(), out adjClose);
                int.TryParse(htmlBody.FirstOrDefault().ElementAtOrDefault(6).ToString(), NumberStyles.AllowThousands, culture, out volume);
            }
            var yahooSummaryModel = new YahooSummaryModel
            {
                Ticker = ticker,
                DateAdded = dateAdded,
                DailyOpen = open,
                DailyClose = close,
                DailyHigh = high,
                DailyLow = low,
                AdjClose = adjClose,
                Volume = volume
            };
            this.logger.Information(yahooSummaryModel.Volume.ToString());
            return yahooSummaryModel;
        }
    }
}
