using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TickerInfoRetrievalService.Models;
using TickerInfoRetrievalService.Services.Interfaces;

namespace TickerInfoRetrievalService.Services
{
    class DBCommunicationService : IDBCommunicationService
    {
        private readonly string dbEndpoint;
        private readonly ILogger logger;
        private static HttpClient client = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan };

        public DBCommunicationService(string dbEndpoint)
        {
            this.dbEndpoint = dbEndpoint;
            this.logger = Log.ForContext<DBCommunicationService>();
        }

        public async Task<IEnumerable<Tickers>> GetTickers()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{this.dbEndpoint}/TickerInfo/GetTickers");
                var responseString = await response.Content.ReadAsStreamAsync();
                var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var tickers = await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<Tickers>>(responseString, jsonOptions);
                return tickers;
            }
        }

        public async Task InsertDailyInfoByTicker(YahooSummaryModel tickerDailyInfo)
        {
             this.logger.Information($"Sending Daily Info For: {tickerDailyInfo.Ticker}");
             var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
             var payload = System.Text.Json.JsonSerializer.Serialize<YahooSummaryModel>(tickerDailyInfo, jsonOptions);
             var stringContent = new StringContent(payload, Encoding.Default, "application/json");
             var response = await client.PostAsync($"{this.dbEndpoint}/TickerInfo/InsertDailyInfo", stringContent);
             if (!response.IsSuccessStatusCode)
             {
                 this.logger.Warning($"Failed to add daily info for {tickerDailyInfo.Ticker}");
             }
        }
    }
}
