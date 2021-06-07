using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace PowerBot.Core
{
    public class TestHandler : BaseHandler
    {
        [MessageReaction(ChatAction.Typing)]
        [MessagePattern("/start")]
        public void Start()
        {
            string messageText = $"Hi! your database id is {User.Id}, telegram id is {User.TelegramId}, chatId is {ChatId}.";
            Bot.SendTextMessageAsync(ChatId, messageText);
        }

        [Role(UserAccess.Admin)]
        [MessageReaction(ChatAction.Typing)]
        [MessagePattern("admin")]
        public void Admin()
        {
            string messageText = $"If You can read this, You are Admin";
            Bot.SendTextMessageAsync(ChatId, messageText);
        }
    }
}
