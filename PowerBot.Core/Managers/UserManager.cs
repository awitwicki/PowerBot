using Microsoft.EntityFrameworkCore;
using PowerBot.Core.Data;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace PowerBot.Core.Managers
{
    public static class UserManager
    {
        public static PowerBotContext _dbContext = new PowerBotContext();

        public static async Task<PowerbotUser> GetUser(long id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            return user;
        }
        
        public static async Task<List<PowerbotUser>> GetUsers()
        {
            var users = await _dbContext.Users.ToListAsync();
            return users;
        }

        public static async Task<PowerbotUser> GetUserByTelegamId(long id)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.TelegramId == id);
            return user;
        }

        public static async Task<PowerbotUser> AddOrUpdateUser(Message message)
        {
            var usr = await GetUserByTelegamId(message.From.Id);

            // New user
            if (usr == null)
            {
                PowerbotUser user = new PowerbotUser
                {
                    TelegramId = message.From.Id,
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                    UserName = message.From.Username,
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
                usr.FirstName = message.From.FirstName;
                usr.LastName = message.From.LastName;
                usr.UserName = message.From.Username;
                usr.ActiveAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();

            return usr;
        }

        public static async Task UpdateUser(PowerbotUser user)
        {
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public static async Task UpdateUserAccess(long userId, UserAccess userAccess)
        {
            var user = await GetUser(userId);
            user.UserAccess = userAccess;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
