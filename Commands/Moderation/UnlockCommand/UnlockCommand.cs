using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace ArtcordBot.Features.ModerationCommands
{
    public partial class ModerationCommandGroup 
    {
        [Command("unlock")]
        [Description("Unlocks a channel or the whole server.")]
        public async Task UnlockAsync(CommandContext ctx, DiscordChannel? channel = null)
        {
            var targetChannel = channel ?? ctx.Channel;
            var everyoneRole = ctx.Guild!.EveryoneRole;
            var message = await _guildSettingsService.GetUnlockMessageAsync(ctx.Guild!.Id);

            await targetChannel.AddOverwriteAsync(everyoneRole, allow: DiscordPermissions.SendMessages);

            if (message is not null)
            {
                await ctx.RespondAsync(message);
            }
            else
            {
                await ctx.RespondAsync($"🔓 {targetChannel.Mention} has been unlocked.");
            }
        }
    }
}
