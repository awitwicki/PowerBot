using PowerBot.Core.Models;
using System;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace PowerBot.Core
{
    public abstract class BaseHandler
    {
        public TelegramBotClient Bot { get; set; }
        public PowerBot.Core.Models.User User { get; set; }
        public MessageEventArgs MessageEventArgs { get; set; }
        public int MessageId => MessageEventArgs.Message.From.Id;
        public long ChatId => MessageEventArgs.Message.Chat.Id;
        public void Init(TelegramBotClient bot, PowerBot.Core.Models.User user, MessageEventArgs messageEventArgs)
        {
            Bot = bot;
            User = user;
            MessageEventArgs = messageEventArgs;
        }

        public async virtual void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            //do nothing
            Console.WriteLine("Background on message received");
        }

        //Methods attributes
        public class MessageReaction : Attribute
        {
            public ChatAction ChatAction { get; set; }
            public MessageReaction(ChatAction chatAction)
            {
                this.ChatAction = chatAction;
            }
        }

        public class Role : Attribute
        {
            public UserAccess UserAccess { get; set; }
            public Role(UserAccess userAccess)
            {
                this.UserAccess = userAccess;
            }
        }

        //Attribute validators
        //Validate user access role
        public static bool ValidateAccess(MethodInfo methodInfo, PowerBot.Core.Models.User user)
        {
            Object[] attributes = methodInfo.GetCustomAttributes(true);

            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(Role))
                {
                    var access = ((Role)attribute).UserAccess;
                    if (user.UserAccess != access)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //Send chat action
        public static Nullable<ChatAction> GetChatActionAttributes(MethodInfo methodInfo)
        {
            Object[] attributes = methodInfo.GetCustomAttributes(true);

            //find and return chatAction
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(MessageReaction))
                {
                    var chatAction = ((MessageReaction)attribute).ChatAction;
                    return chatAction;
                }
            }

            return null;
        }
    }
}

