using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TickerInfoRetrievalService.Models;
using TickerInfoRetrievalService.Services.Interfaces;

namespace TickerInfoRetrievalService.Services
{
    class InfoRetrievalService : IInfoRetrievalService
    {
        private readonly string tickerInfoEndpoint;
        private readonly string apiKey;
        private readonly ILogger logger;

        public InfoRetrievalService(string tickerInfoEndpoint, string apiKey)
        {
            this.tickerInfoEndpoint = tickerInfoEndpoint;
            this.apiKey = apiKey;
            this.logger = Log.ForContext<InfoRetrievalService>();
        }

        public async Task GetDailyInfoByTicker(string ticker) 
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{this.tickerInfoEndpoint}/stable/stock/{ticker}/batch?types=quote,news,chart&range=1m&last=10&token={this.apiKey}");
                response.EnsureSuccessStatusCode();
                var responseStream = await response.Content.ReadAsStreamAsync();
                var jsonOptions = new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                var results = await System.Text.Json.JsonSerializer.DeserializeAsync<DailyInfoModel>(responseStream, jsonOptions);
               // var results = await System.Text.Json.JsonSerializer.DeserializeAsync<DailyInfoModel>(responseStream, jsonOptions);
            }
        }
    }
}
