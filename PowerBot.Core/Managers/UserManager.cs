using Microsoft.EntityFrameworkCore;
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

        public static async Task<User> GetUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            return user;
        }
        
        public static async Task<List<User>> GetUsers()
        {
            var users = await _dbContext.Users.ToListAsync();
            return users;
        }

        public static async Task<User> GetUserByTelegamId(int id)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.TelegramId == id);
            return user;
        }

        public static async Task<User> AddOrUpdateUser(MessageEventArgs message)
        {
            var usr = await GetUserByTelegamId(message.Message.From.Id);

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

                var usrEntity = await _dbContext.Users.AddAsync(user);
                usr = usrEntity.Entity;

                Console.WriteLine($"New user ({user.FullName})");
                await LogsManager.CreateLog($"New user ({user.FullName})", LogLevel.Info);
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

        public static async Task UpdateUser(User user)
        {
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public static async Task UpdateUserAccess(int userId, UserAccess userAccess)
        {
            var user = await GetUser(userId);
            user.UserAccess = userAccess;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
