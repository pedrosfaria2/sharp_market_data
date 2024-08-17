using System.Net.WebSockets;
using System.Threading.Channels;
using System.Text;

namespace BinanceWebSocket.Services
{
    public class WebSocketManager
    {
        private readonly ClientWebSocket _client;
        private readonly SubscriptionManager _subscriptionManager;
        private readonly Channel<string> _messageChannel;
        private readonly MessageProcessor _messageProcessor;

        public WebSocketManager()
        {
            _client = new ClientWebSocket();
            _subscriptionManager = new SubscriptionManager(_client);
            _messageChannel = Channel.CreateUnbounded<string>();

            var databaseWorker = new DatabaseWorker(new DatabaseManager());
            _messageProcessor = new MessageProcessor(_messageChannel, databaseWorker);
        }

        public async Task Start()
        {
            Console.WriteLine("Connecting to WebSocket...");
            if (_client.State != WebSocketState.Open)
            {
                await _client.ConnectAsync(new Uri("wss://stream.binance.com:9443/ws"), CancellationToken.None);
                Console.WriteLine("WebSocket connected");

                if (_client.State == WebSocketState.Open)
                {
                    Console.WriteLine("WebSocket connection confirmed open.");
                    _ = Task.Run(async () => await ReceiveMessagesAsync());
                    _ = Task.Run(async () => await _messageProcessor.ProcessMessagesAsync());
                }
                else
                {
                    Console.WriteLine("Failed to open WebSocket connection.");
                }
            }
            else
            {
                Console.WriteLine("WebSocket is already open.");
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            Console.WriteLine("Starting to receive messages...");
            var buffer = new byte[1024 * 8];

            while (_client.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("WebSocket connection closed.");
                        await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }
                    else
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        //Console.WriteLine($"Message received: {message}");

                        await _messageChannel.Writer.WriteAsync(message);
                    }
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"WebSocket error: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }
        }

        public async Task SubscribeToPair(string pair, List<string> validSymbols)
        {
            await _subscriptionManager.SubscribeToPair(pair, validSymbols);
        }

        public async Task SubscribeToSymbol(string symbol)
        {
            await _subscriptionManager.SubscribeToSymbol(symbol); 
        }

        public async Task Unsubscribe(string symbol)
        {
            await _subscriptionManager.Unsubscribe(symbol);
        }

        public async Task Stop()
        {
            Console.WriteLine("Stopping WebSocket...");
            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            Console.WriteLine("WebSocket stopped.");
        }
    }
}
