using Spectre.Console;
using BinanceWebSocket.Models;
using System;
using System.Collections.Generic;

namespace BinanceWebSocket.Util
{
    public class MarketDataStatsDisplay
    {
        private readonly List<MarketData> _recentTrades = new();
        private MarketDataStats _currentData = new MarketDataStats();

        public void RenderUI()
        {
            AnsiConsole.Live(CreateLayout())
                .Start(ctx =>
                {
                    while (true)
                    {
                        ctx.Refresh();
                        System.Threading.Thread.Sleep(500);
                    }
                });
        }

        public void UpdateMarketData(MarketData data, MarketDataStats stats)
        {
            _recentTrades.Insert(0, data);
            if (_recentTrades.Count > 10)
            {
                _recentTrades.RemoveAt(_recentTrades.Count - 1);
            }

            _currentData = stats;
        }

        private Layout CreateLayout()
        {
            var layout = new Layout("root")
                .SplitRows(
                    new Layout("trades").Size(60),
                    new Layout("stats").SplitColumns(
                        new Layout("stats1"),
                        new Layout("stats2"),
                        new Layout("buyer_maker"),
                        new Layout("performance")
                    )
                );

            layout["trades"].Update(CreateTradesTable());
            layout["stats1"].Update(CreateStatsPanel(1));
            layout["stats2"].Update(CreateStatsPanel(2));
            layout["buyer_maker"].Update(CreateBuyerMakerBarChart());
            layout["performance"].Update(CreatePerformanceStats());

            return layout;
        }

        private Table CreateTradesTable()
        {
            var table = new Table();
            table.AddColumn("Symbol");
            table.AddColumn("Price");
            table.AddColumn("Quantity");
            table.AddColumn("Aggressor");
            table.AddColumn("Time");

            foreach (var trade in _recentTrades)
            {
                table.AddRow(
                    trade.Symbol,
                    trade.Price.ToString("0.00"),
                    trade.Quantity.ToString(string.Empty),
                    trade.IsBuyerMarketMaker ? "[green]Buyer[/]" : "[red]Seller[/]",
                    trade.TradeTime.ToString("HH:mm:ss")
                );
            }

            return table.Border(TableBorder.Rounded)
                .Title("[bold blue]Trades[/]")
                .BorderStyle(new Style(Color.Yellow));
        }

        private Panel CreateStatsPanel(int column)
        {
            var stats = column == 1
                ? new List<string>
                {
                    $"Last Price: {_currentData.LastPrice:0.00}",
                    $"Avg Price: {_currentData.AvgPrice:0.00}",
                    $"Median Price: {_currentData.MedianPrice:0.00}",
                    $"Max Price: {_currentData.MaxPrice:0.00}",
                    $"Min Price: {_currentData.MinPrice:0.00}",
                    $"EMA: {_currentData.Ema:0.00}",
                    $"SMA: {_currentData.Sma:0.00}",
                }
                : new List<string>
                {
                    $"VWAP: {_currentData.VolumeWeightedAvgPrice:0.00}",
                    $"Total Volume: {_currentData.TotalVolume:0.00}",
                    $"Standard Deviation: {_currentData.StdDev:0.00}",
                    $"RSI: {_currentData.Rsi:0.00}"
                };

            var panel = new Panel(string.Join("\n", stats))
                .Header(column == 1 ? "Stats" : "Stats (contd.)")
                .BorderStyle(new Style(Color.Green))
                .Border(BoxBorder.Rounded);

            return panel;
        }

        private BarChart CreateBuyerMakerBarChart()
        {
            var total = _currentData.BuyerMakerTrue + _currentData.BuyerMakerFalse;
            var percentBuyer = total > 0 ? (_currentData.BuyerMakerTrue * 100) / total : 0;
            var percentSeller = 100 - percentBuyer;

            var chart = new BarChart()
                .Width(60)
                .Label("[bold yellow]Buyer vs Seller[/]")
                .AddItem("Buyer", percentBuyer, Color.Green)
                .AddItem("Seller", percentSeller, Color.Red);

            return chart;
        }

        private Panel CreatePerformanceStats()
        {
            var performanceStats = new List<string>
            {
                $"Messages Processed: {_currentData.MessageCount}",
                $"Avg Arrival Interval: {_currentData.AvgArrivalInterval:0.00} ms",
                $"Avg Processing Time: {_currentData.AvgProcessingTime:0.00} ms"
            };

            var panel = new Panel(string.Join("\n", performanceStats))
                .Header("Performance")
                .BorderStyle(new Style(Color.Red))
                .Border(BoxBorder.Rounded);

            return panel;
        }
    }

    public class MarketDataStats
    {
        public double LastPrice { get; set; }
        public double AvgPrice { get; set; }
        public double MedianPrice { get; set; }
        public double MaxPrice { get; set; }
        public double MinPrice { get; set; }
        public double Ema { get; set; }
        public double Sma { get; set; }
        public double VolumeWeightedAvgPrice { get; set; }
        public double TotalVolume { get; set; }
        public double StdDev { get; set; }
        public double Rsi { get; set; }
        public int BuyerMakerTrue { get; set; }
        public int BuyerMakerFalse { get; set; }
        public int MessageCount { get; set; }
        public double AvgArrivalInterval { get; set; }
        public double AvgProcessingTime { get; set; }
    }
}
