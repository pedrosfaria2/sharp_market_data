using System.Data.SQLite;
using BinanceWebSocket.Models;

namespace BinanceWebSocket.Services
{
    public class DatabaseManager
    {
        private readonly SQLiteConnection _connection;

        public DatabaseManager(string dbName = "market_data.db")
        {
            _connection = new SQLiteConnection($"Data Source={dbName};Version=3;");
            _connection.Open();
            CreateTables();
        }

        private void CreateTables()
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS market_data (
                    event_type TEXT,
                    event_time INTEGER,
                    symbol TEXT,
                    trade_id INTEGER,
                    price TEXT,
                    quantity TEXT,
                    trade_time INTEGER,
                    is_buyer_market_maker INTEGER
                )";
            command.ExecuteNonQuery();
            Console.WriteLine("Database tables created or verified.");
        }

        public void InsertData(MarketData data)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText = @"
                INSERT INTO market_data (
                    event_type, event_time, symbol, trade_id, price, quantity, trade_time, is_buyer_market_maker
                ) VALUES (@event_type, @event_time, @symbol, @trade_id, @price, @quantity, @trade_time, @is_buyer_market_maker)";
            command.Parameters.AddWithValue("@event_type", data.EventType);
            command.Parameters.AddWithValue("@event_time", data.EventTime);
            command.Parameters.AddWithValue("@symbol", data.Symbol);
            command.Parameters.AddWithValue("@trade_id", data.TradeId);
            command.Parameters.AddWithValue("@price", data.Price);
            command.Parameters.AddWithValue("@quantity", data.Quantity);
            command.Parameters.AddWithValue("@trade_time", data.TradeTime);
            command.Parameters.AddWithValue("@is_buyer_market_maker", data.IsBuyerMarketMaker);
            command.ExecuteNonQuery();
            //Console.WriteLine("Data inserted into the database.");
        }
    }
}
