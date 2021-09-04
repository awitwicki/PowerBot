using PowerBot.Core.Handlers;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleSample
{
    class SampleHandler : BaseHandler
    {
        [MessageReaction(ChatAction.Typing)]
        [MessagePattern("/test")]
        public async Task Start()
        {
            string messageText = $"Hi! your telegram id is {User.Id}, chatId is {ChatId}.";
            messageText += $"\nThis message was handled in SampleHandler";

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Say hello!", "say_hello"),
                },
            });

            await Bot.SendTextMessageAsync(ChatId, messageText, replyMarkup: inlineKeyboard);
        }
    }
}
