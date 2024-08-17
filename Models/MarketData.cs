namespace BinanceWebSocket.Models
{
    public class MarketData
    {
        public string EventType { get; set; } = string.Empty;
        public long EventTime { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public long TradeId { get; set; }
        public string Price { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public long TradeTime { get; set; }
        public bool IsBuyerMarketMaker { get; set; }
    }
}