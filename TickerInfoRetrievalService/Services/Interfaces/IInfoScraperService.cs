using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickerInfoRetrievalService.Models;

namespace TickerInfoRetrievalService.Services.Interfaces
{
    public interface IInfoScraperService
    {
        Task CreateBrowser();
        #nullable enable
        Task<YahooSummaryModel?> ScrapeByTicker(string ticker);
    }
}
