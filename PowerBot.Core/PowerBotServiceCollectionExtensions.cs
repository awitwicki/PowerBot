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
            services.AddHostedService<PowerBotHostedService>();
            return services;
        }
    }
}
