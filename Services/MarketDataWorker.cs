using System.Threading.Channels;
using BinanceWebSocket.Models;
using Spectre.Console;

namespace BinanceWebSocket.Services
{
    public class MarketDataWorker
    {
        private readonly ChannelReader<MarketData> _marketDataChannel;
        private readonly Dictionary<string, MarketData> _marketDataStorage;

        public MarketDataWorker(ChannelReader<MarketData> marketDataChannel, Dictionary<string, MarketData> marketDataStorage)
        {
            _marketDataChannel = marketDataChannel;
            _marketDataStorage = marketDataStorage;
        }

        public async Task StartViewingAsync(string symbol)
        {
            while (true)
            {
                while (_marketDataChannel.TryRead(out var marketData))
                {
                    if (marketData.Symbol == symbol)
                    {
                        var table = new Table();
                        table.AddColumn("Property");
                        table.AddColumn("Value");

                        table.AddRow("Symbol", marketData.Symbol);
                        table.AddRow("Price", marketData.Price);
                        table.AddRow("Quantity", marketData.Quantity);
                        table.AddRow("Trade ID", marketData.TradeId.ToString());
                        table.AddRow("Trade Time", marketData.TradeTime.ToString());
                        table.AddRow("Buyer is Market Maker", marketData.IsBuyerMarketMaker.ToString());

                        AnsiConsole.Clear();
                        AnsiConsole.Write(table);

                        await Task.Delay(500);
                    }
                }
            }
        }
    }
}