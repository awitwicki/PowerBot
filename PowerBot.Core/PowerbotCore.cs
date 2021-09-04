using Microsoft.Extensions.Configuration;
using PowerBot.Core.Handlers;
using PowerBot.Core.Managers;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PowerBot.Core
{
    public class PowerbotCore
    {
        private static TelegramBotClient Bot { get; set; }
        private CancellationTokenSource cts { get; set; }
        public TelegramBotClient BotClient => Bot;

        public static bool Started { get; set; } = false;
        public string AccessTokenEnvName { get; set; } = "TelegramAccessToken";
        public string TelegramAccessToken { get; set; }

        public PowerbotCore() { }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (cts != null)
                cts.Cancel();

            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            if (TelegramAccessToken == null)
            {
                var TelegramAccessToken = Environment.GetEnvironmentVariable(AccessTokenEnvName);
                if (TelegramAccessToken == null)
                    throw new InvalidOperationException($"Can't find environment variable {AccessTokenEnvName}");
            }

            Bot = new TelegramBotClient(TelegramAccessToken);
            StartBotAsync();
            Started = true;

            await LogsManager.CreateLog($"Bot started", LogLevel.Info);
        }

        public void StartBotAsync()
        {
            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            Bot.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

            LogsManager.CreateLogSync($"Start listening for @{Bot.GetMeAsync().Result.Username}", LogLevel.Info);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Task handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
                //UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery),
                //UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery),
                //UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }
        
        private Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            //Log stats
            await StatsManager.AddStatAction(ActionType.Error);
            await LogsManager.CreateLog(ErrorMessage, LogLevel.Critical);
        }

        async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            //TODO
            //Ignore old messages
            //if (message.Date.AddMinutes(1) < DateTime.Now)
            //    return;

            if (message.Text != null)
                await MessageInvoker(message);
        }

        async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            //TODO
            //Ignore old messages
            //if (message.Date.AddMinutes(1) < DateTime.Now)
            //    return;

            await CallbackQueryInvoker(callbackQuery);
        }

        private async Task MessageInvoker(Message message)
        {
            //Log stats
            await StatsManager.AddStatAction(ActionType.Message);

            //Get message data
            var user = await UserManager.AddOrUpdateUser(message.From);
            var chat = await ChatManager.AddOrUpdateChat(message);

            //Get all handlers
            var handlers = ReflectiveEnumerator.GetEnumerableOfType<BaseHandler>();

            foreach (var handlerType in handlers)
            {
                //Find method in handler
                MethodInfo[] handlerMethods = handlerType.GetMethods();

                foreach (var method in handlerMethods)
                {
                    //Pattern matching for message text
                    if (BaseHandler.MatchMethod(method, message.Text))
                    {
                        //Check user access by role
                        if (!BaseHandler.ValidateAccess(method, user))
                            return;

                        try
                        {
                            //Get and send chatAction from attributes
                            var chatAction = BaseHandler.GetChatActionAttributes(method);
                            if (chatAction.HasValue)
                                await Bot.SendChatActionAsync(message.Chat.Id, chatAction.Value);

                            //Cast handler object
                            var handler = Activator.CreateInstance(handlerType);

                            //Set params
                            ((BaseHandler)handler).Init(Bot, user, message: message);

                            //Invoke method
                            await (Task)method.Invoke(handler, parameters: new object[] { });
                        }
                        catch (Exception ex)
                        {
                            await LogsManager.CreateLog($"Invoker error *{ex.Message}*", LogLevel.Critical);

                            //Log stats
                            await StatsManager.AddStatAction(ActionType.Error);
                        }
                    }
                    else
                    {
                        //Cant find method
                        //await LogsManager.CreateLog($"Can't find method for *{messageEventArgs.Message.Text}*", LogLevel.Warning);
                        //Console.WriteLine($"Can't find method for *{messageEventArgs.Message.Text}*");
                    }
                }
            }
        }

        private async Task CallbackQueryInvoker(CallbackQuery callbackQuery)
        {
            //Log stats
            await StatsManager.AddStatAction(ActionType.CallbackQuery);

            //Get message data
            var user = await UserManager.AddOrUpdateUser(callbackQuery.From);

            //Get all handlers
            var handlers = ReflectiveEnumerator.GetEnumerableOfType<BaseCallbackQueryHandler>();

            foreach (var handlerType in handlers)
            {
                //Find method in handler
                MethodInfo[] handlerMethods = handlerType.GetMethods();

                foreach (var method in handlerMethods)
                {
                    //Pattern matching for message text
                    if (BaseHandler.MatchMethod(method, callbackQuery.Data))
                    {
                        //Check user access by role
                        if (!BaseHandler.ValidateAccess(method, user))
                            return;

                        try
                        {
                            //Get and send chatAction from attributes
                            var chatAction = BaseHandler.GetChatActionAttributes(method);
                            if (chatAction.HasValue)
                                await Bot.SendChatActionAsync(callbackQuery.Message.Chat.Id, chatAction.Value);

                            //Cast handler object
                            var handler = Activator.CreateInstance(handlerType);

                            //Set params
                            ((BaseHandler)handler).Init(Bot, user, callbackQuery: callbackQuery);

                            //Invoke method
                            await (Task)method.Invoke(handler, parameters: new object[] { });
                        }
                        catch (Exception ex)
                        {
                            await LogsManager.CreateLog($"Invoker error *{ex.Message}*", LogLevel.Critical);

                            //Log stats
                            await StatsManager.AddStatAction(ActionType.Error);
                        }
                    }
                    else
                    {
                        //Cant find method
                        //await LogsManager.CreateLog($"Can't find method for *{messageEventArgs.Message.Text}*", LogLevel.Warning);
                        //Console.WriteLine($"Can't find method for *{messageEventArgs.Message.Text}*");
                    }
                }
            }
        }
    }
}
