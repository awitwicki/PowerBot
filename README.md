# PowerBot
ASP.NET Core 5.0 [Telegram bot](https://github.com/TelegramBots/Telegram.Bot) wrapper.

```diff
- This project is in early stage!
```

## How to use

1. Add package to your project

2. Create class that inherits `BaseHandler` class and define bot methods:

```csharp
    class SampleHandler : BaseHandler
    {
        [MessageReaction(ChatAction.Typing)]
        [MessagePattern("/start")]
        public async Task Start()
        {
            string messageText = $"Hi! your id is {User.TelegramId}, chatId is {ChatId}.";
            
            await Bot.SendTextMessageAsync(ChatId, messageText);
        }

        [MessageReaction(ChatAction.Typing)]
        [Role(UserAccess.Admin)]
        [MessagePattern("/test")]
        public async Task TestMethod()
        {
            string messageText = $"This method avaliable only for admins";

            await Bot.SendTextMessageAsync(ChatId, messageText);
        }
    }
```

3. In `Main` define your PowerBot configuration:

```csharp
    using PowerBot.Web;

    static void Main(string[] args)
    {
        PowerBotBuilder
            .BuildPowerBot()
            .WithAccessToken("TELEGRAM_BOT_TOKEN")
            .StartWithWebServer();
    }
```

Also you can use telegram access token from environment variables with default name `TelegramAccessToken` or define your own env variable with different name and configure it in builder:

```csharp
    .WithAccessTokenEnv("myOwnEnvVariableName")
```
## Web admin panel

Also PowerBot gives you the ability to manage your bot through a web interface. By defaults PowerBot deploys a local web server with database (SQLite). Web application is based on ASP.NET Core and Blazor.

You can view the statistics of using the bot and edit access for your bot's users.
