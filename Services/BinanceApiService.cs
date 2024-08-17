using Newtonsoft.Json.Linq;

namespace BinanceWebSocket.Services
{
    public class BinanceApiService
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task<List<string>> FetchSymbols()
        {
            const string url = "https://api.binance.com/api/v3/exchangeInfo";
            var response = await Client.GetStringAsync(url);
            var data = JObject.Parse(response);
            var symbols = new List<string>();

            if (data["symbols"] is not JArray symbolArray) return symbols;
            symbols.AddRange(symbolArray.Select(symbol => symbol["symbol"]?.ToString().ToLower()).Where(symbolName => !string.IsNullOrEmpty(symbolName))!);

            return symbols;
        }
    }
}