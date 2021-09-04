using PowerBot.Core.Handlers;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace PowerBot.Core
{
    public class TestHandler : BaseCallbackQueryHandler
    {
        [MessageReaction(ChatAction.Typing)]
        [MessagePattern("say_hello")]
        public async Task Hello()
        {
            await Bot.AnswerCallbackQueryAsync(CallbackQuery.Id, "Hello", true);
        }
    }
}
