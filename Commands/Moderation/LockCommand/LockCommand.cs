using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace ArtcordBot.Features.ModerationCommands
{
    public partial class ModerationCommandGroup 
    {
        [Command("lock")]
        [Description("Locks a channel or the whole server.")]
        public async Task LockAsync(CommandContext ctx, DiscordChannel? channel = null)
        {
            var targetChannel = channel ?? ctx.Channel;
            var everyoneRole = ctx.Guild!.EveryoneRole;
            var message = await _guildSettingsService.GetLockMessageAsync(ctx.Guild!.Id);

            await targetChannel.AddOverwriteAsync(everyoneRole, deny: DiscordPermissions.SendMessages);

            if (message is not null)
            {
                await ctx.RespondAsync(message);
            }
            else
            {
                await ctx.RespondAsync($"🔒 {targetChannel.Mention} has been locked.");
            }
        }
    }
}