using System;
using PowerBot.Web;

namespace ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            PowerBotBuilder
                .BuildPowerBot()
                .WithAccessToken("TELEGRAM_BOT_TOKEN")
                .WithPassword("admin")
                .StartWithWebServer();
        }
    }
}
