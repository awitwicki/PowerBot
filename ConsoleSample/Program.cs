using System;
using PowerBot.Core;

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
                .StartSyncrously();
        }
    }
}
