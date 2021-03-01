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
        private static TelegramBotClient Bot { get; set; }
        public TelegramBotClient BotClient => Bot;

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
            Bot.OnReceiveError += BotOnReceiveError;

            //Start listening
            Bot.StartReceiving(Array.Empty<UpdateType>());
        }

        async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            var checkMessageResult = await CheckMessage(messageEventArgs);
            if (!checkMessageResult)
                return;

            if (message.Text != null)
                MessageInvoker(messageEventArgs);
        }

        private async void MessageInvoker(MessageEventArgs messageEventArgs)
        {
            //Get message data
            var chatId = messageEventArgs.Message.Chat.Id;
            var user = await UserManager.AddOrUpdateUser(messageEventArgs);
            
            string methodName = messageEventArgs.Message.Text;

            //Get all handlers
            var handlers = ReflectiveEnumerator.GetEnumerableOfType<BaseHandler>();

            foreach (var handlerType in handlers)
            {
                //Find method in handler
                MethodInfo method = handlerType.GetMethod(methodName);
                if (method != null)
                {
                    //Check user access by role
                    if (!BaseHandler.ValidateAccess(method, user))
                        return;

                    try
                    {
                        //Get and send chatAction from attributes
                        var chatAction = BaseHandler.GetChatActionAttributes(method);
                        if (chatAction.HasValue)
                            await Bot.SendChatActionAsync(chatId, chatAction.Value);

                        //Cast handler object
                        var handler = Activator.CreateInstance(handlerType);

                        //Set params
                        ((BaseHandler)handler).Init(Bot, user, messageEventArgs);

                        //Invoke method
                        method.Invoke(handler, parameters: new object[] { });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex, "Invoker error");
                    }
                }
                else
                {
                    //Cant find method
                    Debug.WriteLine($"Can't find method *{methodName}*");
                }
            }
        }
        private async Task<bool> CheckMessage(MessageEventArgs messageEventArgs)
        {
            // Process user in db
            var user = await UserManager.AddOrUpdateUser(messageEventArgs);

            //Filters for messages
            if (messageEventArgs != null)
            {
                var message = messageEventArgs.Message;
                if (message == null || (message.Type != MessageType.Text && message.Type != MessageType.Document)) return false;

                //Ignore old messages
                if (message.Date.AddMinutes(1) < DateTime.UtcNow)
                {
                    return false;
                }
            }

            //Authorize User (if user banned - ignore)
            if (user.IsBanned)
            {
                return false;
            }

            return true;
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Debug.WriteLine(receiveErrorEventArgs.ApiRequestException.ErrorCode);
            Debug.WriteLine(receiveErrorEventArgs.ApiRequestException.Message);
        }
    }
}
