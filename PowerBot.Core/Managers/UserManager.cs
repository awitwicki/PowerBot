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
        public static async Task<PowerbotUser> AddOrUpdateUser(User user)
        {
            var usr = await GetUser(user.Id);

            // New user
            if (usr == null)
            {
                PowerbotUser _user = new PowerbotUser
                {
                    Id = user.Id,
                    ActiveAt = DateTime.UtcNow,
                    UserAccess = UserAccess.User,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.Username
                };

                var usrEntity = await _dbContext.Users.AddAsync(_user);
                usr = usrEntity.Entity;

                await LogsManager.CreateLog($"New user ({_user.FullName})", LogLevel.Info);
            }
            // Update User
            else
            {
                usr.FirstName = user.FirstName;
                usr.LastName = user.LastName;
                usr.UserName = user.Username;
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
