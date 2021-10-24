using PowerBot.Core.Handlers;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace PowerBot.Core
{
    public class TestHandler : BaseHandler
    {
        [MessageReaction(ChatAction.Typing)]
        [MessagePattern("/start")]
        public async Task Start()
        {
            string messageText = $"Hi! your telegram id is {User.Id}, chatId is {ChatId}.";
            await Bot.SendTextMessageAsync(ChatId, messageText);
        }

        [Role(UserAccess.Admin)]
        [MessageReaction(ChatAction.Typing)]
        [MessagePattern("admin")]
        public async Task Admin()
        {
            string messageText = $"If You can read this, You are Admin";
            await Bot.SendTextMessageAsync(ChatId, messageText);
        }
    }
}
