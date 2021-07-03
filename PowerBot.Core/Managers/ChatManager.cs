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
    public static class ChatManager
    {
        public static PowerBotContext _dbContext = new PowerBotContext();

        public static async Task<Chat> GetChat(long id)
        {
            var chat = await _dbContext.Chats.FindAsync(id);
            return chat;
        }

        public static async Task<List<Chat>> GetChats()
        {
            var chats = await _dbContext.Chats.ToListAsync();
            return chats;
        }

        public static async Task<Chat> AddOrUpdateChat(MessageEventArgs message)
        {
            var chatFromDb = await GetChat(message.Message.Chat.Id);

            // New Сhat
            if (chatFromDb == null)
            {
                Chat chat = new Chat
                {
                    Id = message.Message.Chat.Id,
                    Title = message.Message.Chat.Title,
                    ActiveAt = DateTime.UtcNow,
                };

                var chatEntity = await _dbContext.Chats.AddAsync(chat);
                chatFromDb = chatEntity.Entity;

                Console.WriteLine($"New Chat ({chatFromDb.Title})");
                await LogsManager.CreateLog($"New Chat ({chatFromDb.Title})", LogLevel.Info);
            }
            // Update Сhat
            else
            {
                chatFromDb.Title = message.Message.From.FirstName;
                chatFromDb.ActiveAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();

            return chatFromDb;
        }

        public static async Task UpdateChat(Chat chat)
        {
            _dbContext.Update(chat);
            await _dbContext.SaveChangesAsync();
        }
    }
}
