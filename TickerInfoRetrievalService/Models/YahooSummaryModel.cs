using System;
using System.Collections.Generic;
using System.Text;

namespace TickerInfoRetrievalService.Models
{
    class YahooSummaryModel
    {
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal AdjClose { get; set; }
        public int Volume { get; set; }
    }
}
