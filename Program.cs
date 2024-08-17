using BinanceWebSocket.CLI;

namespace BinanceWebSocket
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting the User Interface...");
            var ui = new UserInterface();
            await ui.Start();
        }
    }
}