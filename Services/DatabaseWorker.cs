using BinanceWebSocket.Models;


namespace BinanceWebSocket.Services
{
    public class DatabaseWorker(DatabaseManager dbManager)
    {
        public async Task InsertDataAsync(MarketData marketData)
        {
            //Console.WriteLine($"Inserting data for {marketData.Symbol} into database.");
            await Task.Run(() => dbManager.InsertData(marketData));
            //Console.WriteLine($"Data inserted for {marketData.Symbol}");
        }
    }
}