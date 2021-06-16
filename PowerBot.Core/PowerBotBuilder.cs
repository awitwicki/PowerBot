using PowerBot.Core.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace PowerBot.Core
{
    public class PowerBotBuilder
    {
        private static PowerbotCore PowerbotCore { get; set; }
        public static PowerBotBuilder BuildPowerBot()
        {
            return new PowerBotBuilder();
        }

        public PowerBotBuilder()
        {
            PowerbotCore = new PowerbotCore();
        }

        public PowerBotBuilder WithAccessToken(string accessToken)
        {
            PowerbotCore.TelegramAccessToken = accessToken;
            return this;
        }

        public PowerBotBuilder WithAccessTokenEnv(string accessTokenEnvName)
        {
            PowerbotCore.AccessTokenEnvName = accessTokenEnvName;
            return this;
        }

        public void StartSyncrously()
        {
            PowerbotCore
                .StartAsync(new System.Threading.CancellationToken())
                .GetAwaiter()
                .GetResult();

            Task.Delay(-1).Wait();
        }
    }
}
