using System.Threading.Channels;
using Newtonsoft.Json.Linq;
using BinanceWebSocket.Models;

namespace BinanceWebSocket.Services
{
    public class MessageProcessor(Channel<string> messageChannel, DatabaseWorker databaseWorker)
    {
        public async Task ProcessMessagesAsync()
        {
            while (await messageChannel.Reader.WaitToReadAsync())
            {
                while (messageChannel.Reader.TryRead(out var message))
                {
                    var data = JObject.Parse(message);

                    if (data?["e"]?.ToString() == "trade")
                    {
                        //Console.WriteLine($"Processing trade event for symbol: {data?["s"]?.ToString()}");

                        var marketData = new MarketData
                        {
                            EventType = data?["e"]?.ToString() ?? string.Empty,
                            EventTime = long.Parse(data?["E"]?.ToString() ?? "0"),
                            Symbol = data?["s"]?.ToString() ?? string.Empty,
                            TradeId = long.Parse(data?["t"]?.ToString() ?? "0"),
                            Price = data?["p"]?.ToString() ?? string.Empty,
                            Quantity = data?["q"]?.ToString() ?? string.Empty,
                            TradeTime = long.Parse(data?["T"]?.ToString() ?? "0"),
                            IsBuyerMarketMaker = bool.Parse(data?["m"]?.ToString() ?? "false")
                        };

                        await databaseWorker.InsertDataAsync(marketData);
                    }
                    else
                    {
                        Console.WriteLine("Received a message that is not a trade event.");
                    }
                }
            }
        }
    }
}
