using Microsoft.EntityFrameworkCore;
using PowerBot.Core.Data;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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

        public static async Task<PowerbotUser> AddOrUpdateUser(Message message)
        {
            var usr = await GetUser(message.From.Id);

            // New user
            if (usr == null)
            {
                PowerbotUser user = new PowerbotUser
                {
                    Id = message.From.Id,
                    ActiveAt = DateTime.UtcNow,
                    UserAccess = UserAccess.User,
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                    UserName = message.From.Username
                };

                var usrEntity = await _dbContext.Users.AddAsync(user);
                usr = usrEntity.Entity;

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
