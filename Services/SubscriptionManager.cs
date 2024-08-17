using System.Net.WebSockets;
using System.Text;
using BinanceWebSocket.Util;


namespace BinanceWebSocket.Services
{
    public class SubscriptionManager(ClientWebSocket client)
    {
        private readonly List<string> _subscriptions = [];

        public async Task SubscribeToPair(string pair, List<string> validSymbols)
        {
            var matchingSymbols = validSymbols.Where(s => s.EndsWith(pair.ToLower())).ToList();

            if (matchingSymbols.Any())
            {
                var subscribeMessage = new
                {
                    method = "SUBSCRIBE",
                    @params = matchingSymbols.Select(s => $"{s}@trade").ToArray(),
                    id = 1
                };

                var message = Encoding.UTF8.GetBytes(JsonHelper.Serialize(subscribeMessage));
                await client.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine($"Subscribed to symbols: {string.Join(", ", matchingSymbols)}");

                _subscriptions.AddRange(matchingSymbols);
            }
            else
            {
                Console.WriteLine($"No symbols found for pair '{pair}'.");
            }
        }

        public async Task Unsubscribe(string symbol)
        {
            if (_subscriptions.Contains(symbol))
            {
                var unsubscribeMessage = new
                {
                    method = "UNSUBSCRIBE",
                    @params = new[] { $"{symbol}@trade" },
                    id = 2
                };

                var message = Encoding.UTF8.GetBytes(JsonHelper.Serialize(unsubscribeMessage));
                await client.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine($"Unsubscribed from {symbol}@trade");
                _subscriptions.Remove(symbol);
            }
            else
            {
                Console.WriteLine($"Symbol {symbol} not found in subscriptions.");
            }
        }
    }
}
