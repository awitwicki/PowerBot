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
            //do nothing
            return host;
        }

        public static IHostBuilder WithAccessTokenEnv(this IHostBuilder host, string accessTokenEnvName)
        {
            //do nothing
            return host;
        }

        public static void StartWithWebServer(this IHostBuilder host)
        {
            host.Build().Run();
        }
    }
}
