using System;
using System.Collections.Generic;
using System.Text;
using TickerInfoRetrievalService.Models;

namespace TickerInfoRetrievalService.Models
{
    class DailyInfoModel
    {
        public IEnumerable<ChartModel> chart { get; set; }
        public IEnumerable<NewsModel>  news { get; set; }
        public IEnumerable<QuoteModel> quote { get; set; }
    }
}
