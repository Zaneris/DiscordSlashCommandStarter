using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Discord.Commands.Services;
using Remora.Discord.Core;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace zBot.Services
{
    public class BotService : IHostedService
    {
        private readonly DiscordGatewayClient _discordGatewayClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly SlashService _slashService;
        private Task<Result> _clientTask;
        private readonly CancellationTokenSource _clientCancellation = new();

        public BotService(DiscordGatewayClient discordGatewayClient, ILogger<Program> logger, IConfiguration config,
            SlashService slashService)
        {
            _discordGatewayClient = discordGatewayClient;
            _logger = logger;
            _config = config;
            _slashService = slashService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Token: {_config["zBot:Token"]}");
            _logger.LogInformation($"Guild ID: {_config["zBot:GuildId"]}");
            Snowflake? debugServer = null;
#if DEBUG
            var debugServerString = _config["zBot:GuildId"];
            if (debugServerString is not null)
            {
                if (!Snowflake.TryParse(debugServerString, out debugServer))
                {
                    _logger.LogWarning("Failed to parse guild ID.");
                    throw new Exception("Guild ID required.");
                }
            }
#endif
            var checkSlashSupport = _slashService.SupportsSlashCommands();
            if (!checkSlashSupport.IsSuccess)
            {
                _logger.LogWarning
                (
                    "The registered commands of the bot don't support slash commands: {Reason}",
                    checkSlashSupport.Error.Message
                );
                throw new Exception("Unable to continue.");
            }
            else
            {
                var updateSlash = await _slashService.UpdateSlashCommandsAsync(debugServer, cancellationToken);
                if (!updateSlash.IsSuccess)
                {
                    _logger.LogWarning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
                    throw new Exception("Unable to continue.");
                }
            }
            
            _clientTask = _discordGatewayClient.RunAsync(_clientCancellation.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _clientCancellation.Cancel();
            var runResult = await _clientTask;
            if (!runResult.IsSuccess)
            {
                switch (runResult.Error)
                {
                    case ExceptionError exe:
                    {
                        _logger.LogError
                        (
                            exe.Exception,
                            "Exception during gateway connection: {ExceptionMessage}",
                            exe.Message
                        );

                        break;
                    }
                    case GatewayWebSocketError:
                    case GatewayDiscordError:
                    {
                        _logger.LogError("Gateway error: {Message}", runResult.Error.Message);
                        break;
                    }
                    default:
                    {
                        _logger.LogError("Unknown error: {Message}", runResult.Error.Message);
                        break;
                    }
                }
            }
            _logger.LogInformation("Goodbye!");
        }
    }
}
