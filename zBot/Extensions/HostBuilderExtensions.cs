using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Remora.Discord.Caching.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway.Extensions;
using zBot.Helpers;
using zBot.Services;

namespace zBot.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder CreateBotHostDefaults(this IHostBuilder builder, Action<BotHostBuilder> configure)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
#if DEBUG
            builder.UseEnvironment(Environments.Development);
            configBuilder.AddJsonFile("appsettings.development.json");
            try
            {
                configBuilder.AddUserSecrets<Program>();
            }
            catch (InvalidOperationException e) { }
#endif
            configBuilder.AddEnvironmentVariables();
            var config = configBuilder.Build();
            builder.ConfigureAppConfiguration(appConfig =>
            {
                appConfig.Sources.Clear();
                appConfig.AddConfiguration(config);
            });

            builder.ConfigureServices(serviceCollection =>
            {
                serviceCollection
                    .AddDiscordGateway(_ => config["zBot:Token"])
                    .AddDiscordCommands(true);
                configure.Invoke(new BotHostBuilder(config, serviceCollection));
                serviceCollection
                    .AddDiscordCaching()
                    .AddHostedService<BotService>();
            });
            
            return builder;
        }
    }
}