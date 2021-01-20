using HtmlAgilityPack;
using PuppeteerSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TickerInfoRetrievalService.Models;

namespace TickerInfoRetrievalService.Services
{
    class InfoScraperService
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
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync($"https://finance.yahoo.com/quote/{ticker}/history?p={ticker}");
                string content = await page.GetContentAsync();
                return ParseHtml(content);
            }
        }

        private YahooSummaryModel ParseHtml(string html)
        {
            var htmlDoc = new HtmlDocument(); 
            htmlDoc.LoadHtml(html);
            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//table")
                .Descendants("tr")
                .Skip(1)
                .Where(tr => tr.Elements("td").Count() > 1)
                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                .ToList();
            try
            {
                var yahooSummaryModel = new YahooSummaryModel
                {
                    Date = DateTime.Parse(htmlBody.FirstOrDefault()[0]),
                    Open = decimal.Parse(htmlBody.FirstOrDefault()[1]),
                    High = decimal.Parse(htmlBody.FirstOrDefault()[2]),
                    Low = decimal.Parse(htmlBody.FirstOrDefault()[3]),
                    Close = decimal.Parse(htmlBody.FirstOrDefault()[4]),
                    AdjClose = decimal.Parse(htmlBody.FirstOrDefault()[5]),
                    Volume = int.Parse(htmlBody.FirstOrDefault()[6], System.Globalization.NumberStyles.AllowThousands)
                };
                return yahooSummaryModel;
            }
            catch (NullReferenceException e)
            {
                return new YahooSummaryModel();
            }
            
        }
    }
}
