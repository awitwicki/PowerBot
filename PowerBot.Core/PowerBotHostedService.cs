using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PowerBot.Core.Managers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace PowerBot.Core
{
    public class PowerBotHostedService : IHostedService
    {
        private static TelegramBotClient Bot { get; set; }

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
            var token = _configuration["TelegramAccessToken"];
            if (token == null)
                throw new InvalidOperationException("Can't find TelegramAccessToken");

            Bot = new TelegramBotClient(token);
            StartBotAsync();
            Started = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Bot.StopReceiving();

            return Task.CompletedTask;
        }
        public void StartBotAsync()
        {
            Bot.OnMessage += BotOnMessageReceived;

            //Start listening
            Bot.StartReceiving(Array.Empty<UpdateType>());
        }

        async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            // Process user in db
            var user = await UserManager.AddOrUpdateUser(messageEventArgs);

            // Exmaple
            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            if (message.Text != null)
                await Bot.SendTextMessageAsync(message.Chat.Id, message.Text);
        }
    }
}
