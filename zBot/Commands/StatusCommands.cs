using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Results;

namespace zBot.Commands
{
    public class StatusCommands : CommandGroup
    {
        private readonly IDiscordRestWebhookAPI _webhookApi;
        private readonly InteractionContext _interactionContext;

        public StatusCommands(IDiscordRestWebhookAPI webhookApi, InteractionContext interactionContext)
        {
            _webhookApi = webhookApi;
            _interactionContext = interactionContext;
        }

        [Command("ping")]
        [Description("Ping the bot!")]
        public async Task<IResult> PostPongStatusAsync()
        {
            var embed = new Embed(Description: "Pong!", Colour: Color.LawnGreen);
            var reply = await _webhookApi.CreateFollowupMessageAsync
            (
                _interactionContext.ApplicationID,
                _interactionContext.Token,
                embeds: new[] {embed},
                ct: CancellationToken
            );
            return !reply.IsSuccess
                ? Result.FromError(reply)
                : Result.FromSuccess();
        }
    }
}