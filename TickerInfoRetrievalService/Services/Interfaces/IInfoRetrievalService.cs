using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TickerInfoRetrievalService.Services.Interfaces
{
    public interface IInfoRetrievalService
    {
        Task GetDailyInfoByTicker(string ticker);
    }
}
