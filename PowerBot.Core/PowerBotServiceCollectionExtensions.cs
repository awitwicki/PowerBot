using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerBot.Core
{
    public static class PowerBotServiceCollectionExtensions
    {
        public static IServiceCollection AddPowerBot(this IServiceCollection services)
        {
            services.AddSingleton<PowerBotHostedService>();
            services.AddHostedService(provider => provider.GetService<PowerBotHostedService>());
            return services;
        }
    }
}
