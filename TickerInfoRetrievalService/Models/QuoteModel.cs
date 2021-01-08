using System;
using System.Collections.Generic;
using System.Text;

namespace TickerInfoRetrievalService.Models
{
    class QuoteModel
    {
         public string symbol { get; set; }
         public string companyName { get; set; }
         public string primaryExchange { get; set; }
         public string calculationPrice { get; set; }
         public float open { get; set; }
         public long openTime { get; set; }
         public string openSource { get; set; }
         public float close { get; set; }
         public long closeTime { get; set; }
         public string closeSource { get; set; }
         public float high { get; set; }
         public long highTime { get; set; }
         public string highSource { get; set; }
         public float low { get; set; }
         public long lowTime { get; set; }
         public string lowSource { get; set; }
         public float latestPrice { get; set; }
         public string latestSource { get; set; }
         public string latestTime { get; set; }
         public long latestUpdate { get; set; }
         public int latestVolume { get; set; }
         public float iexRealtimePrice { get; set; }
         public int iexRealtimeSize { get; set; }
         public long iexLastUpdated { get; set; }
         public float delayedPrice { get; set; }
         public long delayedPriceTime { get; set; }
         public float oddLotDelayedPrice { get; set; }
         public long oddLotDelayedPriceTime { get; set; }
         public float extendedPrice { get; set; }
         public float extendedChange { get; set; }
         public float extendedChangePercent { get; set; }
         public long extendedPriceTime { get; set; }
         public float previousClose { get; set; }
         public int previousVolume { get; set; }
         public float change { get; set; }
         public float changePercent { get; set; }
         public int volume { get; set; }
         public float iexMarketPercent { get; set; }
         public int iexVolume { get; set; }
         public int avgTotalVolume { get; set; }
         public int iexBidPrice { get; set; }
         public int iexBidSize { get; set; }
         public int iexAskPrice { get; set; }
         public int iexAskSize { get; set; }
         public float iexOpen { get; set; }
         public long iexOpenTime { get; set; }
         public float iexClose { get; set; }
         public long iexCloseTime { get; set; }
         public long marketCap { get; set; }
         public float peRatio { get; set; }
         public float week52High { get; set; }
         public float week52Low { get; set; }
         public float ytdChange { get; set; }
         public long lastTradeTime { get; set; }
         public bool isUSMarketOpen { get; set; }
    }
}
