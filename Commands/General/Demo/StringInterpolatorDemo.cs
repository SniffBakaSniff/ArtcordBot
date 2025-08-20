using System.ComponentModel;
using ArtcordBot.Services;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace ArtcordBot.Features.GeneralCommands
{
    public class StringInterpolatorDemo
    {
        private readonly IStringInterpolatorService _stringInterpolatorService;

        public StringInterpolatorDemo(IStringInterpolatorService stringInterpolatorService)
        {
            _stringInterpolatorService = stringInterpolatorService;
        }

        [Command("interpolate")]
        [Description("Interpolates a string with context-specific values")]
        public async Task ExecuteAsync(CommandContext ctx, [Description("The string to interpolate")] string template)
        {
            var eventContext = new EventContext(ctx);
            var interpolatedString = _stringInterpolatorService.Interpolate(template, eventContext);

            var embed = new DiscordEmbedBuilder
            {
                Title = "String Interpolation",
                Color = DiscordColor.Blurple
            }
            .AddField("Original", template, false)
            .AddField("Interpolated", interpolatedString, false)
            .Build();

            await ctx.RespondAsync(embed: embed);
        }
    }
}