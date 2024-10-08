﻿using Spectre.Console;
using BinanceWebSocket.Services;

namespace BinanceWebSocket.CLI
{
    public class UserInterface
    {
        private readonly WebSocketManager _wsManager = new();
        private readonly BinanceApiService _apiService = new();

        public async Task Start()
        {
            var validSymbols = await _apiService.FetchSymbols();

            while (true)
            {
                AnsiConsole.Clear();
                
                AnsiConsole.Write(
                    new FigletText("Binance WebSocket")
                        .Centered()
                        .Color(Color.Gold1));

                AnsiConsole.Write(
                    new Panel("[green]Select an option below to continue[/]")
                        .Border(BoxBorder.Rounded)
                        .Header("[bold gold3]Main Menu[/]", Justify.Center)
                        .HeaderAlignment(Justify.Center)
                        .Padding(1, 1)
                        .BorderStyle(new Style(Color.Gold3))
                        .RoundedBorder());

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]What would you like to do?[/]")
                        .PageSize(8)
                        .HighlightStyle(new Style(Color.Gold1))
                        .MoreChoicesText("[grey](Move up and down to select an option)[/]")
                        .AddChoices(new[]
                        {
                            "1. [bold aqua]Start[/] WebSocket",
                            "2. [bold aqua]Stop[/] WebSocket",
                            "3. [bold aqua]Subscribe[/] to Symbols by Pair",
                            "4. [bold aqua]Unsubscribe[/] from Symbol",
                            "5. [bold aqua]Fetch[/] Symbols",
                            "6. [bold aqua]Subscribe[/] to a Single Symbol",
                            "7. [bold red]Exit[/]"
                        }));

                switch (choice)
                {
                    case "1. [bold aqua]Start[/] WebSocket":
                        AnsiConsole.MarkupLine("[green]Starting WebSocket...[/]");
                        await _wsManager.Start();
                        break;
                    case "2. [bold aqua]Stop[/] WebSocket":
                        AnsiConsole.MarkupLine("[green]Stopping WebSocket...[/]");
                        await _wsManager.Stop();
                        break;
                    case "3. [bold aqua]Subscribe[/] to Symbols by Pair":
                        var pair = AnsiConsole.Ask<string>("[yellow]Enter pair (e.g., usdt):[/]");
                        await _wsManager.SubscribeToPair(pair, validSymbols);
                        break;
                    case "4. [bold aqua]Unsubscribe[/] from Symbol":
                        var symbol = AnsiConsole.Ask<string>("[yellow]Enter symbol:[/]");
                        await _wsManager.Unsubscribe(symbol);
                        break;
                    case "5. [bold aqua]Fetch[/] Symbols":
                        validSymbols = await _apiService.FetchSymbols();
                        
                        int columns = 5;

                        var grid = new Grid();
                        for (int i = 0; i < columns; i++)
                        {
                            grid.AddColumn();
                        }
                        
                        for (int i = 0; i < validSymbols.Count; i += columns)
                        {
                            var row = new List<string>();
                            for (int j = 0; j < columns; j++)
                            {
                                row.Add(i + j < validSymbols.Count ? $"[aqua]{validSymbols[i + j]}[/]" : "");
                            }
                            grid.AddRow(row.ToArray());
                        }

                        AnsiConsole.Write(grid);
                        break;
                    case "6. [bold aqua]Subscribe[/] to a Single Symbol":
                        validSymbols = await _apiService.FetchSymbols();

                        var selectedSymbol = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[yellow]Select a symbol to subscribe to:[/]")
                                .PageSize(10)
                                .HighlightStyle(new Style(Color.Aqua))
                                .AddChoices(validSymbols));

                        await _wsManager.SubscribeToSymbol(selectedSymbol);
                        break;
                    case "7. [bold red]Exit[/]":
                        AnsiConsole.MarkupLine("[green]Exiting...[/]");
                        return;
                }

                AnsiConsole.MarkupLine("\n[grey]Press any key to return to the main menu...[/]");
                Console.ReadKey(true);
            }
        }
    }
}
