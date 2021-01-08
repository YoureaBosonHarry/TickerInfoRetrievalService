using System;
using System.Collections.Generic;
using System.Text;

namespace TickerInfoRetrievalService.Models
{
    public class ChartModel
    {
        public float close { get; set; }
        public float high { get; set; }
        public float low { get; set; }
        public float open { get; set; }
        public string symbol { get; set; }
        public int volume { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string subkey { get; set; }
        public string date { get; set; }
        public long updated { get; set; }
        public int changeOverTime { get; set; }
        public int marketChangeOverTime { get; set; }
        public float uOpen { get; set; }
        public float uClose { get; set; }
        public float uHigh { get; set; }
        public float uLow { get; set; }
        public int uVolume { get; set; }
        public float fOpen { get; set; }
        public float fClose { get; set; }
        public float fHigh { get; set; }
        public float fLow { get; set; }
        public int fVolume { get; set; }
        public string label { get; set; }
        public int change { get; set; }
        public int changePercent { get; set; }
    }

}
