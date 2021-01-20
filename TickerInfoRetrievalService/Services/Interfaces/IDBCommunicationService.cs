using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickerInfoRetrievalService.Models;

namespace TickerInfoRetrievalService.Services.Interfaces
{
    public interface IDBCommunicationService
    {
        Task<IEnumerable<Tickers>> GetTickers();
    }
}
