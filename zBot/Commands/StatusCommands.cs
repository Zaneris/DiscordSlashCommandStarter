using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;

namespace zBot.Commands
{
    public class StatusCommands : CommandGroup
    {
        private readonly FeedbackService _feedbackService;

        public StatusCommands(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [Command("ping")]
        [Description("Ping the bot!")]
        public async Task<IResult> PostPongStatusAsync()
        {
            var embed = new Embed(Description: "Pong!", Colour: Color.LawnGreen);
            var reply = await _feedbackService.SendContextualEmbedAsync(embed, this.CancellationToken);
            return !reply.IsSuccess
                ? Result.FromError(reply)
                : Result.FromSuccess();
        }
    }
}
