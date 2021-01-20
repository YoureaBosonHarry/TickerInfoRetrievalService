using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TickerInfoRetrievalService.Models;
using TickerInfoRetrievalService.Services.Interfaces;

namespace TickerInfoRetrievalService.Services
{
    class DBCommunicationService : IDBCommunicationService
    {
        private readonly string dbEndpoint;

        public DBCommunicationService(string dbEndpoint)
        {
            this.dbEndpoint = dbEndpoint;
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
    }
}
