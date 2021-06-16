using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PowerBot.Core.Managers;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace PowerBot.Core
{
    public class PowerBotHostedService : IHostedService
    {
        private static PowerbotCore PowerbotCore { get; set; }
        public TelegramBotClient BotClient => PowerbotCore.BotClient;

        private readonly IServiceProvider _scopeFactory;
        private readonly IConfiguration _configuration;

        public static bool Started { get; set; } = false;

        public PowerBotHostedService(
            IServiceProvider scopeFactory,
            IConfiguration configuration
            )
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            PowerbotCore = new PowerbotCore();

            var token = _configuration["TelegramAccessToken"];
            if (token != null)
                PowerbotCore.TelegramAccessToken = token;

            PowerbotCore.StartAsync(stoppingToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            PowerbotCore.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
