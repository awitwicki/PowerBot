using PowerBot.Core.Models;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PowerBot.Core
{
    public abstract class BaseHandler
    {
        public TelegramBotClient Bot { get; set; }
        public PowerBot.Core.Models.PowerbotUser User { get; set; }
        public Message Message { get; set; }
        public long MessageId => Message.From.Id;
        public long ChatId => Message.Chat.Id;
        public void Init(TelegramBotClient bot, PowerbotUser user, Message message)
        {
            Bot = bot;
            User = user;
            Message = message;
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

        public class MessagePattern : Attribute
        {
            public string Pattern { get; set; }
            public MessagePattern(string pattern)
            {
                this.Pattern = pattern;
            }
        }

        //Attribute validators
        //Validate user access role
        public static bool ValidateAccess(MethodInfo methodInfo, PowerbotUser user)
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

        //Match message text with method
        public static bool MatchMethod(MethodInfo methodInfo, string messageText)
        {
            Object[] attributes = methodInfo.GetCustomAttributes(true);

            //find and return chatAction
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(MessagePattern))
                {
                    var pattern = ((MessagePattern)attribute).Pattern;

                    //regex match
                    Match m = Regex.Match(messageText, pattern, RegexOptions.IgnoreCase);
                    return m.Success;
                }
            }

            return false;
        }
    }
}

