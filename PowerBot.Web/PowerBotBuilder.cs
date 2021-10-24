using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerBot.Web
{
    public static class PowerBotBuilder
    {
        public static IHostBuilder BuildPowerBot()
        {
            return PowerBot.Web.Program.CreateHostBuilder(null);
        }

        public static IHostBuilder WithAccessToken(this IHostBuilder host, string accessToken)
        {
            host.ConfigureAppConfiguration((context, config) =>
                {
                    IConfigurationRoot configuration = config.Build();
                    configuration["TelegramAccessToken"] = accessToken;
                });
            
            return host;
        }

        public static IHostBuilder WithAccessTokenEnv(this IHostBuilder host, string accessTokenEnvName)
        {
            host.ConfigureAppConfiguration((context, config) =>
                {
                    IConfigurationRoot configuration = config.Build();
                    configuration["TelegramAccessTokenEnvName"] = accessTokenEnvName;
                });

            return host;
        }

        public static IHostBuilder WithPassword(this IHostBuilder host, string password)
        {
            host.ConfigureAppConfiguration((context, config) =>
            {
                IConfigurationRoot configuration = config.Build();
                configuration["password"] = password;
            });

            return host;
        }

        public static IHostBuilder WithPasswordEnv(this IHostBuilder host, string passwordEnvName)
        {
            string password = Environment.GetEnvironmentVariable(passwordEnvName) ??
                throw new InvalidOperationException($"Can't find environment variable {passwordEnvName}");
            
            host.ConfigureAppConfiguration((context, config) =>
            {
                IConfigurationRoot configuration = config.Build();
                configuration["password"] = password;
            });

            return host;
        }

        public static void StartWithWebServer(this IHostBuilder host)
        {
            host.Build().Run();
        }
    }
}
