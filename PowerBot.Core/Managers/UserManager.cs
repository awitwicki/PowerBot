using PowerBot.Core.Data;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace PowerBot.Core.Managers
{
    public static class UserManager
    {
        public static PowerBotContext _dbContext = new PowerBotContext();

        public static User GetUser(int id)
        {
            return _dbContext.Users.Find(id);
        }
        
        public static List<User> GetUsers()
        {
            return _dbContext.Users.ToList();
        }

        public static User GetUserByTelegamId(int id)
        {
            return _dbContext.Users.SingleOrDefault(x => x.TelegramId == id);
        }

        public static async Task<User> AddOrUpdateUser(MessageEventArgs message)
        {
            var usr = GetUserByTelegamId(message.Message.From.Id);

            // New user
            if (usr == null)
            {
                User user = new User
                {
                    TelegramId = message.Message.From.Id,
                    FirstName = message.Message.From.FirstName,
                    LastName = message.Message.From.LastName,
                    UserName = message.Message.From.Username,
                    ActiveAt = DateTime.UtcNow,
                    UserAccess = UserAccess.User
                };

                usr = _dbContext.Users.Add(user).Entity;
            }
            // Update User
            else
            {
                usr.FirstName = message.Message.From.FirstName;
                usr.LastName = message.Message.From.LastName;
                usr.UserName = message.Message.From.Username;
                usr.ActiveAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();

            return usr;
        }

        public static async void UpdateUser(User user)
        {
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
